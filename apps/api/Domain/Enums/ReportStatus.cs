namespace CommentableAPI.Domain.Enums;

/// <summary>
/// Defines the status of a report
///
/// TypeScript Equivalent: ReportStatus enum in @commentable/shared-types
/// </summary>
public enum ReportStatus
{
    Pending,
    Reviewed,
    Resolved,
    Dismissed
}
