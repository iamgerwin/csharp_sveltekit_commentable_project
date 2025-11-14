import { ReactionType } from '@commentable/shared-enums';
import { BaseEntity } from './BaseEntity';

/**
 * Reaction Entity
 *
 * Represents a user's reaction to a comment
 */
export interface Reaction extends BaseEntity {
  userId: string;
  commentId: string;
  type: ReactionType;
}

/**
 * Reaction summary for a comment
 */
export interface ReactionSummary {
  commentId: string;
  reactions: {
    type: ReactionType;
    count: number;
  }[];
  totalCount: number;
  userReaction?: ReactionType | null; // Current user's reaction
}
