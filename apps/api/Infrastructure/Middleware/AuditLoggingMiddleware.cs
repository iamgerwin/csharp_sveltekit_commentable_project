using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace CommentableAPI.Infrastructure.Middleware;

/// <summary>
/// HIPAA-compliant audit logging middleware
/// Logs all API access for compliance with HIPAA Audit Controls requirement
/// </summary>
public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;

    public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var auditLog = new AuditLogEntry
        {
            Timestamp = DateTime.UtcNow,
            UserId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous",
            UserName = context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Anonymous",
            IPAddress = GetClientIPAddress(context),
            HttpMethod = context.Request.Method,
            Path = context.Request.Path,
            QueryString = context.Request.QueryString.Value,
            UserAgent = context.Request.Headers["User-Agent"].ToString()
        };

        // Capture the original response body stream
        var originalBodyStream = context.Response.Body;

        try
        {
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            auditLog.StatusCode = context.Response.StatusCode;
            auditLog.Success = context.Response.StatusCode < 400;

            // Log the audit entry
            LogAuditEntry(auditLog);

            // Reset the position to the beginning before copying
            responseBody.Seek(0, SeekOrigin.Begin);

            // Copy the response body back to the original stream
            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            auditLog.Success = false;
            auditLog.ErrorMessage = ex.Message;
            auditLog.StatusCode = 500;

            LogAuditEntry(auditLog);
            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private void LogAuditEntry(AuditLogEntry entry)
    {
        // HIPAA requires: Who accessed what, when, and from where
        var logMessage = new StringBuilder();
        logMessage.AppendLine("=== AUDIT LOG ===");
        logMessage.AppendLine($"Timestamp: {entry.Timestamp:O}");
        logMessage.AppendLine($"User ID: {entry.UserId}");
        logMessage.AppendLine($"User Name: {entry.UserName}");
        logMessage.AppendLine($"IP Address: {entry.IPAddress}");
        logMessage.AppendLine($"Action: {entry.HttpMethod} {entry.Path}{entry.QueryString}");
        logMessage.AppendLine($"Status: {entry.StatusCode}");
        logMessage.AppendLine($"Success: {entry.Success}");
        if (!string.IsNullOrEmpty(entry.ErrorMessage))
        {
            logMessage.AppendLine($"Error: {entry.ErrorMessage}");
        }
        logMessage.AppendLine("=================");

        if (entry.Success)
        {
            _logger.LogInformation(logMessage.ToString());
        }
        else
        {
            _logger.LogWarning(logMessage.ToString());
        }

        // In production, this should also write to a dedicated audit log database
        // that is append-only and has integrity protection (HIPAA requirement)
    }

    private string GetClientIPAddress(HttpContext context)
    {
        // Check for forwarded IP first (behind proxy/load balancer)
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues forwardedFor))
        {
            var ip = forwardedFor.FirstOrDefault();
            if (!string.IsNullOrEmpty(ip))
            {
                // Take the first IP if there are multiple
                return ip.Split(',')[0].Trim();
            }
        }

        // Check X-Real-IP header
        if (context.Request.Headers.TryGetValue("X-Real-IP", out StringValues realIp))
        {
            var ip = realIp.FirstOrDefault();
            if (!string.IsNullOrEmpty(ip))
            {
                return ip;
            }
        }

        // Fall back to RemoteIpAddress
        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}

/// <summary>
/// Audit log entry model
/// </summary>
public class AuditLogEntry
{
    public DateTime Timestamp { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? QueryString { get; set; }
    public string UserAgent { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Extension method to easily add the middleware to the pipeline
/// </summary>
public static class AuditLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseAuditLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuditLoggingMiddleware>();
    }
}
