namespace CommentableAPI.Domain.Enums;

/// <summary>
/// Defines the types of entities that can have comments.
/// This supports polymorphic relationships in the comment system.
///
/// TypeScript Equivalent: CommentableType enum in @commentable/shared-enums
/// </summary>
public enum CommentableType
{
    Video,
    Post
}
