import { EntityStatus } from '@commentable/shared-enums';
import { BaseEntity } from './BaseEntity';

/**
 * Post Entity
 *
 * Represents a blog post or social media post that can receive comments
 */
export interface Post extends BaseEntity {
  title: string;
  content: string;
  excerpt?: string | null;
  featuredImageUrl?: string | null;
  userId: string;
  status: EntityStatus;
  viewCount: number;
  commentCount: number;
  publishedAt?: Date | null;
}
