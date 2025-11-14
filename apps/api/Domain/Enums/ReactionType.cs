namespace CommentableAPI.Domain.Enums;

/// <summary>
/// Defines the types of reactions users can give to comments.
/// Supports multiple reaction types beyond simple like/dislike.
///
/// TypeScript Equivalent: ReactionType enum in @commentable/shared-enums
/// </summary>
public enum ReactionType
{
    Like,
    Dislike,
    Love,
    Clap,
    Laugh,
    Sad
}
