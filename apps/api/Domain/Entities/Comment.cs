using CommentableAPI.Domain.Enums;

namespace CommentableAPI.Domain.Entities;

/// <summary>
/// Comment entity with polymorphic relationships
/// Supports nested comments (replies) and can be attached to different entity types
/// </summary>
public class Comment : BaseEntity
{
    /// <summary>
    /// Content of the comment
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to the user who created the comment
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Type of entity this comment belongs to (Video or Post)
    /// </summary>
    public CommentableType CommentableType { get; set; }

    /// <summary>
    /// ID of the entity this comment belongs to (polymorphic reference)
    /// </summary>
    public Guid CommentableId { get; set; }

    /// <summary>
    /// Foreign key to parent comment (for nested comments/replies)
    /// </summary>
    public Guid? ParentCommentId { get; set; }

    /// <summary>
    /// Entity status (Active, Deleted, Flagged, Removed)
    /// </summary>
    public EntityStatus Status { get; set; } = EntityStatus.Active;

    /// <summary>
    /// Number of reactions (denormalized for performance)
    /// </summary>
    public int ReactionCount { get; set; }

    /// <summary>
    /// Number of replies (denormalized for performance)
    /// </summary>
    public int ReplyCount { get; set; }

    // Navigation properties

    /// <summary>
    /// The user who created this comment
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Parent comment if this is a reply
    /// </summary>
    public Comment? ParentComment { get; set; }

    /// <summary>
    /// Replies to this comment
    /// </summary>
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();

    /// <summary>
    /// Reactions to this comment
    /// </summary>
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();

    /// <summary>
    /// Reports filed against this comment
    /// </summary>
    public ICollection<Report> Reports { get; set; } = new List<Report>();
}
