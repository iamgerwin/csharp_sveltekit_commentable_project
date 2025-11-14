using CommentableAPI.Domain.Enums;

namespace CommentableAPI.Domain.Entities;

/// <summary>
/// Report entity for content moderation
/// Allows users to report inappropriate comments
/// </summary>
public class Report : BaseEntity
{
    /// <summary>
    /// Foreign key to the user who filed the report
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Foreign key to the comment being reported
    /// </summary>
    public Guid CommentId { get; set; }

    /// <summary>
    /// Category of the report (Spam, Harassment, HateSpeech, etc.)
    /// </summary>
    public ReportCategory Reason { get; set; }

    /// <summary>
    /// Additional description provided by the reporter
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Status of the report (Pending, Reviewed, Resolved, Dismissed)
    /// </summary>
    public ReportStatus Status { get; set; } = ReportStatus.Pending;

    /// <summary>
    /// When the report was reviewed by a moderator
    /// </summary>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>
    /// Foreign key to the moderator who reviewed the report
    /// </summary>
    public Guid? ReviewedBy { get; set; }

    /// <summary>
    /// Notes from the moderator about the review
    /// </summary>
    public string? ReviewNotes { get; set; }

    // Navigation properties

    /// <summary>
    /// The user who filed this report
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// The comment that was reported
    /// </summary>
    public Comment Comment { get; set; } = null!;
}
