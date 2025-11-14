using CommentableAPI.Domain.Enums;

namespace CommentableAPI.Domain.Entities;

/// <summary>
/// Post entity that can receive comments
/// </summary>
public class Post : BaseEntity
{
    /// <summary>
    /// Title of the post
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Content of the post
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// URL-friendly slug for the post
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// URL to the featured image
    /// </summary>
    public string? FeaturedImageUrl { get; set; }

    /// <summary>
    /// Number of comments (denormalized for performance)
    /// </summary>
    public int CommentCount { get; set; }

    /// <summary>
    /// Number of views (denormalized for performance)
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// Entity status (Active, Deleted, Flagged, Removed)
    /// </summary>
    public EntityStatus Status { get; set; } = EntityStatus.Active;

    /// <summary>
    /// When the post was published
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// Foreign key to the user who created the post
    /// </summary>
    public Guid UserId { get; set; }

    // Navigation properties

    /// <summary>
    /// The user who created this post
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Comments on this post
    /// </summary>
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
