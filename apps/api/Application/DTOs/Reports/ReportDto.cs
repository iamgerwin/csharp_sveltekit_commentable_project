using CommentableAPI.Domain.Enums;

namespace CommentableAPI.Application.DTOs.Reports;

/// <summary>
/// Report Data Transfer Object
/// </summary>
public class ReportDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ReporterUsername { get; set; } = string.Empty;
    public Guid CommentId { get; set; }
    public string CommentContent { get; set; } = string.Empty;
    public ReportCategory Category { get; set; }
    public string? Description { get; set; }
    public ReportStatus ReportStatus { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedBy { get; set; }
    public string? ReviewerUsername { get; set; }
    public string? ReviewNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Create Report Request
/// </summary>
public class CreateReportDto
{
    public Guid CommentId { get; set; }
    public ReportCategory Category { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Review Report Request
/// </summary>
public class ReviewReportDto
{
    public ReportStatus ReportStatus { get; set; }
    public string? ReviewNotes { get; set; }
}

/// <summary>
/// Report Filter Parameters
/// </summary>
public class ReportFilterDto
{
    public ReportStatus? ReportStatus { get; set; }
    public ReportCategory? Category { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "createdAt";
    public string SortOrder { get; set; } = "desc";
}
