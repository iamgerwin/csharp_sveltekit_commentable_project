namespace CommentableAPI.Infrastructure.Middleware;

/// <summary>
/// Middleware to add security headers for HIPAA, GDPR, and general security compliance
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add security headers BEFORE the response is generated
        // HIPAA Technical Safeguards: Access Control - Encryption and Decryption
        context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";

        // Prevent clickjacking attacks (HIPAA Technical Safeguards)
        context.Response.Headers["X-Frame-Options"] = "DENY";

        // Prevent MIME-sniffing
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";

        // XSS Protection
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

        // Content Security Policy - HIPAA compliant CSP
        var cspPolicy = "default-src 'self'; " +
                       "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                       "style-src 'self' 'unsafe-inline'; " +
                       "img-src 'self' data: https:; " +
                       "font-src 'self' data:; " +
                       "connect-src 'self' http://localhost:5173 http://localhost:5174 http://localhost:3000; " +
                       "frame-ancestors 'none'; " +
                       "base-uri 'self'; " +
                       "form-action 'self'; " +
                       "upgrade-insecure-requests;";
        context.Response.Headers["Content-Security-Policy"] = cspPolicy;

        // Permissions Policy (formerly Feature-Policy)
        context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=(), payment=(), usb=()";

        // Referrer Policy - Don't leak information
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // Cache control for sensitive data (HIPAA requirement)
        // Skip /api/stats endpoint as it has explicit caching via [ResponseCache]
        if (context.Request.Path.StartsWithSegments("/api") &&
            !context.Request.Path.StartsWithSegments("/api/stats"))
        {
            context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, private";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Expires"] = "0";
        }

        await _next(context);
    }
}

/// <summary>
/// Extension method to easily add the middleware to the pipeline
/// </summary>
public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
