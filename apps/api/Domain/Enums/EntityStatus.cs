namespace CommentableAPI.Domain.Enums;

/// <summary>
/// Defines the status of entities (comments, posts, videos, etc.)
/// Used for soft deletion and content moderation.
///
/// Status Flow:
/// Active -> (user deletes) -> Deleted
/// Active -> (moderator flags) -> Flagged -> (moderator reviews) -> Active | Removed
/// Active -> (auto-moderation) -> Flagged
///
/// TypeScript Equivalent: EntityStatus enum in @commentable/shared-enums
/// </summary>
public enum EntityStatus
{
    Active,
    Deleted,
    Flagged,
    Removed
}
