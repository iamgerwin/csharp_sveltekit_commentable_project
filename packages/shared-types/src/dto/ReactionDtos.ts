import { ReactionType } from '@commentable/shared-enums';

/**
 * Create/Update Reaction DTO
 */
export interface UpsertReactionDto {
  commentId: string;
  type: ReactionType;
}

/**
 * Reaction Query Parameters
 */
export interface ReactionQueryParams {
  commentId?: string;
  userId?: string;
  type?: ReactionType;
}
