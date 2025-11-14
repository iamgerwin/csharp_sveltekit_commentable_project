import { CommentableType, EntityStatus } from '@commentable/shared-enums';
import { BaseEntity } from './BaseEntity';
import { UserProfile } from './User';

/**
 * Comment Entity
 *
 * Represents a comment on a commentable entity (Video, Post, etc.)
 * Supports polymorphic relationships via commentableType and commentableId
 */
export interface Comment extends BaseEntity {
  content: string;
  userId: string;
  commentableType: CommentableType;
  commentableId: string;
  parentCommentId?: string | null; // For nested comments/replies
  status: EntityStatus;
  reactionCount: number;
  replyCount: number;
  editedAt?: Date | null;
}

/**
 * Comment with user details (for display)
 */
export interface CommentWithUser extends Comment {
  user: UserProfile;
  replies?: CommentWithUser[];
}
