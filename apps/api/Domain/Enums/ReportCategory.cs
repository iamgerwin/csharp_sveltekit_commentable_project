namespace CommentableAPI.Domain.Enums;

/// <summary>
/// Defines the categories for reporting inappropriate comments.
/// Helps moderators quickly categorize and prioritize reports.
///
/// TypeScript Equivalent: ReportCategory enum in @commentable/shared-enums
/// </summary>
public enum ReportCategory
{
    Spam,
    Harassment,
    HateSpeech,
    Violence,
    SexualContent,
    Misinformation,
    Other
}
