using CommentableAPI.Application.DTOs.Reports;
using CommentableAPI.Domain.Entities;
using CommentableAPI.Domain.Enums;
using CommentableAPI.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CommentableAPI.Presentation.Controllers;

/// <summary>
/// Reports Controller for content moderation
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(ApplicationDbContext context, ILogger<ReportsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of reports (Moderators and Admins only)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<ReportDto>>> GetReports(
        [FromQuery] ReportStatus? reportStatus = null,
        [FromQuery] ReportCategory? category = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string sortBy = "createdAt",
        [FromQuery] string sortOrder = "desc")
    {
        var userId = GetUserId();
        var user = await _context.Users.FindAsync(userId);

        if (user == null || (user.Role != UserRole.Moderator && user.Role != UserRole.Admin))
        {
            return Forbid();
        }

        var query = _context.Reports
            .Include(r => r.User)
            .Include(r => r.Comment)
            .AsQueryable();

        // Apply filters
        if (reportStatus.HasValue)
        {
            query = query.Where(r => r.Status == reportStatus.Value);
        }

        if (category.HasValue)
        {
            query = query.Where(r => r.Reason == category.Value);
        }

        // Apply sorting
        query = sortBy.ToLower() switch
        {
            "createdat" => sortOrder.ToLower() == "asc"
                ? query.OrderBy(r => r.CreatedAt)
                : query.OrderByDescending(r => r.CreatedAt),
            "updatedat" => sortOrder.ToLower() == "asc"
                ? query.OrderBy(r => r.UpdatedAt)
                : query.OrderByDescending(r => r.UpdatedAt),
            "reportstatus" => sortOrder.ToLower() == "asc"
                ? query.OrderBy(r => r.Status)
                : query.OrderByDescending(r => r.Status),
            _ => query.OrderByDescending(r => r.CreatedAt)
        };

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var reports = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ReportDto
            {
                Id = r.Id,
                UserId = r.UserId,
                ReporterUsername = r.User.Username,
                CommentId = r.CommentId,
                CommentContent = r.Comment.Content,
                Category = r.Reason,
                Description = r.Description,
                ReportStatus = r.Status,
                ReviewedAt = r.ReviewedAt,
                ReviewedBy = r.ReviewedBy,
                ReviewerUsername = r.ReviewedBy != null
                    ? _context.Users.Where(u => u.Id == r.ReviewedBy).Select(u => u.Username).FirstOrDefault()
                    : null,
                ReviewNotes = r.ReviewNotes,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return Ok(new PaginatedResponse<ReportDto>
        {
            Data = reports,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        });
    }

    /// <summary>
    /// Get report by ID (Moderators and Admins only)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ReportDto>> GetReport(Guid id)
    {
        var userId = GetUserId();
        var user = await _context.Users.FindAsync(userId);

        if (user == null || (user.Role != UserRole.Moderator && user.Role != UserRole.Admin))
        {
            return Forbid();
        }

        var report = await _context.Reports
            .Include(r => r.User)
            .Include(r => r.Comment)
            .Where(r => r.Id == id)
            .FirstOrDefaultAsync();

        if (report == null)
        {
            return NotFound(new { message = "Report not found" });
        }

        var reportDto = new ReportDto
        {
            Id = report.Id,
            UserId = report.UserId,
            ReporterUsername = report.User.Username,
            CommentId = report.CommentId,
            CommentContent = report.Comment.Content,
            Category = report.Reason,
            Description = report.Description,
            ReportStatus = report.Status,
            ReviewedAt = report.ReviewedAt,
            ReviewedBy = report.ReviewedBy,
            ReviewerUsername = report.ReviewedBy != null
                ? await _context.Users.Where(u => u.Id == report.ReviewedBy).Select(u => u.Username).FirstOrDefaultAsync()
                : null,
            ReviewNotes = report.ReviewNotes,
            CreatedAt = report.CreatedAt,
            UpdatedAt = report.UpdatedAt
        };

        return Ok(reportDto);
    }

    /// <summary>
    /// Create a new report
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ReportDto>> CreateReport([FromBody] CreateReportDto dto)
    {
        var userId = GetUserId();

        // Check if comment exists
        var comment = await _context.Comments.FindAsync(dto.CommentId);
        if (comment == null)
        {
            return NotFound(new { message = "Comment not found" });
        }

        // Check if user already reported this comment
        var existingReport = await _context.Reports
            .Where(r => r.UserId == userId && r.CommentId == dto.CommentId)
            .FirstOrDefaultAsync();

        if (existingReport != null)
        {
            return BadRequest(new { message = "You have already reported this comment" });
        }

        var report = new Report
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CommentId = dto.CommentId,
            Reason = dto.Category,
            Description = dto.Description,
            Status = ReportStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        var user = await _context.Users.FindAsync(userId);

        var reportDto = new ReportDto
        {
            Id = report.Id,
            UserId = report.UserId,
            ReporterUsername = user?.Username ?? "Unknown",
            CommentId = report.CommentId,
            CommentContent = comment.Content,
            Category = report.Reason,
            Description = report.Description,
            ReportStatus = report.Status,
            ReviewedAt = report.ReviewedAt,
            ReviewedBy = report.ReviewedBy,
            ReviewerUsername = null,
            ReviewNotes = report.ReviewNotes,
            CreatedAt = report.CreatedAt,
            UpdatedAt = report.UpdatedAt
        };

        return CreatedAtAction(nameof(GetReport), new { id = report.Id }, reportDto);
    }

    /// <summary>
    /// Review a report (Moderators and Admins only)
    /// </summary>
    [HttpPut("{id}/review")]
    public async Task<ActionResult<ReportDto>> ReviewReport(Guid id, [FromBody] ReviewReportDto dto)
    {
        var userId = GetUserId();
        var user = await _context.Users.FindAsync(userId);

        if (user == null || (user.Role != UserRole.Moderator && user.Role != UserRole.Admin))
        {
            return Forbid();
        }

        var report = await _context.Reports
            .Include(r => r.User)
            .Include(r => r.Comment)
            .Where(r => r.Id == id)
            .FirstOrDefaultAsync();

        if (report == null)
        {
            return NotFound(new { message = "Report not found" });
        }

        report.Status = dto.ReportStatus;
        report.ReviewNotes = dto.ReviewNotes;
        report.ReviewedBy = userId;
        report.ReviewedAt = DateTime.UtcNow;
        report.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var reportDto = new ReportDto
        {
            Id = report.Id,
            UserId = report.UserId,
            ReporterUsername = report.User.Username,
            CommentId = report.CommentId,
            CommentContent = report.Comment.Content,
            Category = report.Reason,
            Description = report.Description,
            ReportStatus = report.Status,
            ReviewedAt = report.ReviewedAt,
            ReviewedBy = report.ReviewedBy,
            ReviewerUsername = user.Username,
            ReviewNotes = report.ReviewNotes,
            CreatedAt = report.CreatedAt,
            UpdatedAt = report.UpdatedAt
        };

        return Ok(reportDto);
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdClaim != null ? Guid.Parse(userIdClaim) : Guid.Empty;
    }
}
