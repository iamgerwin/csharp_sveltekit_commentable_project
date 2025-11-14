/**
 * CommentableType Enum
 *
 * Defines the types of entities that can have comments.
 * This supports polymorphic relationships in the comment system.
 *
 * Usage Example:
 * - Video: A video entity that can receive comments
 * - Post: A blog post or social media post that can receive comments
 *
 * C# Equivalent: CommentableType enum in API.Domain.Enums
 */
export enum CommentableType {
  Video = 'Video',
  Post = 'Post',
}

/**
 * Helper function to validate if a string is a valid CommentableType
 */
export function isValidCommentableType(value: string): value is CommentableType {
  return Object.values(CommentableType).includes(value as CommentableType);
}

/**
 * Get all commentable types as an array
 */
export function getAllCommentableTypes(): CommentableType[] {
  return Object.values(CommentableType);
}
