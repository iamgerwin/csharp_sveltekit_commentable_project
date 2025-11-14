import { CommentableType, EntityStatus } from '@commentable/shared-enums';
import { Comment, CommentWithUser } from '../entities';

/**
 * Create Comment DTO
 */
export interface CreateCommentDto {
  content: string;
  commentableType: CommentableType;
  commentableId: string;
  parentCommentId?: string | null;
}

/**
 * Update Comment DTO
 */
export interface UpdateCommentDto {
  content: string;
}

/**
 * Comment Query Parameters
 */
export interface CommentQueryParams {
  commentableType?: CommentableType;
  commentableId?: string;
  userId?: string;
  status?: EntityStatus;
  parentCommentId?: string | null;
  page?: number;
  pageSize?: number;
  sortBy?: 'createdAt' | 'updatedAt' | 'reactionCount';
  sortOrder?: 'asc' | 'desc';
}

/**
 * Comment Response DTO
 */
export interface CommentResponseDto extends Comment {
  user: {
    id: string;
    username: string;
    avatarUrl?: string | null;
  };
}

/**
 * Comment Tree Response (with nested replies)
 */
export interface CommentTreeResponseDto extends CommentResponseDto {
  replies: CommentTreeResponseDto[];
}
