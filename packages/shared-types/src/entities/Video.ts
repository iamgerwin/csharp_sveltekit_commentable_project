import { EntityStatus } from '@commentable/shared-enums';
import { BaseEntity } from './BaseEntity';

/**
 * Video Entity
 *
 * Represents a video that can receive comments
 */
export interface Video extends BaseEntity {
  title: string;
  description?: string | null;
  url: string;
  thumbnailUrl?: string | null;
  duration: number; // in seconds
  userId: string;
  status: EntityStatus;
  viewCount: number;
  commentCount: number;
}
