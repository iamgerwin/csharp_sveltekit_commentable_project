/**
 * ReactionType Enum
 *
 * Defines the types of reactions users can give to comments.
 * Supports multiple reaction types beyond simple like/dislike.
 *
 * Usage Example:
 * - Like: Standard positive reaction
 * - Dislike: Standard negative reaction
 * - Love: Strong positive reaction (similar to Facebook's Love reaction)
 * - Clap: Appreciation reaction (similar to Medium's clap)
 * - Laugh: Humorous reaction
 * - Sad: Empathetic negative reaction
 *
 * C# Equivalent: ReactionType enum in API.Domain.Enums
 */
export enum ReactionType {
  Like = 'Like',
  Dislike = 'Dislike',
  Love = 'Love',
  Clap = 'Clap',
  Laugh = 'Laugh',
  Sad = 'Sad',
}

/**
 * Reaction metadata for UI rendering
 */
export interface ReactionMetadata {
  type: ReactionType;
  emoji: string;
  label: string;
  color: string;
}

/**
 * Get metadata for all reaction types
 */
export function getReactionMetadata(): Record<ReactionType, ReactionMetadata> {
  return {
    [ReactionType.Like]: {
      type: ReactionType.Like,
      emoji: '👍',
      label: 'Like',
      color: '#3b82f6', // blue
    },
    [ReactionType.Dislike]: {
      type: ReactionType.Dislike,
      emoji: '👎',
      label: 'Dislike',
      color: '#ef4444', // red
    },
    [ReactionType.Love]: {
      type: ReactionType.Love,
      emoji: '❤️',
      label: 'Love',
      color: '#ec4899', // pink
    },
    [ReactionType.Clap]: {
      type: ReactionType.Clap,
      emoji: '👏',
      label: 'Clap',
      color: '#10b981', // green
    },
    [ReactionType.Laugh]: {
      type: ReactionType.Laugh,
      emoji: '😄',
      label: 'Laugh',
      color: '#f59e0b', // yellow
    },
    [ReactionType.Sad]: {
      type: ReactionType.Sad,
      emoji: '😢',
      label: 'Sad',
      color: '#6366f1', // indigo
    },
  };
}

/**
 * Helper function to validate if a string is a valid ReactionType
 */
export function isValidReactionType(value: string): value is ReactionType {
  return Object.values(ReactionType).includes(value as ReactionType);
}

/**
 * Get all reaction types as an array
 */
export function getAllReactionTypes(): ReactionType[] {
  return Object.values(ReactionType);
}
