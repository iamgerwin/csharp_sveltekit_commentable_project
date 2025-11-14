using CommentableAPI.Domain.Enums;

namespace CommentableAPI.Domain.Entities;

/// <summary>
/// Video entity that can receive comments
/// </summary>
public class Video : BaseEntity
{
    /// <summary>
    /// Title of the video
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Description of the video
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// URL to the video file
    /// </summary>
    public string VideoUrl { get; set; } = string.Empty;

    /// <summary>
    /// URL to the video thumbnail image
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Duration of the video in seconds
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// Number of views (denormalized for performance)
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// Number of comments (denormalized for performance)
    /// </summary>
    public int CommentCount { get; set; }

    /// <summary>
    /// Entity status (Active, Deleted, Flagged, Removed)
    /// </summary>
    public EntityStatus Status { get; set; } = EntityStatus.Active;

    /// <summary>
    /// Foreign key to the user who uploaded the video
    /// </summary>
    public Guid UserId { get; set; }

    // Navigation properties

    /// <summary>
    /// The user who uploaded this video
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Comments on this video
    /// </summary>
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
