using CommentableAPI.Domain.Enums;

namespace CommentableAPI.Domain.Entities;

/// <summary>
/// Reaction entity representing user reactions to comments
/// </summary>
public class Reaction : BaseEntity
{
    /// <summary>
    /// Foreign key to the user who created the reaction
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Foreign key to the comment being reacted to
    /// </summary>
    public Guid CommentId { get; set; }

    /// <summary>
    /// Type of reaction (Like, Dislike, Love, Clap, Laugh, Sad)
    /// </summary>
    public ReactionType ReactionType { get; set; }

    // Navigation properties

    /// <summary>
    /// The user who created this reaction
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// The comment this reaction belongs to
    /// </summary>
    public Comment Comment { get; set; } = null!;
}
