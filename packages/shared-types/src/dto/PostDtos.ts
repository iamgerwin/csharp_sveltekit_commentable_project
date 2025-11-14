import { EntityStatus } from '@commentable/shared-enums';

/**
 * Create Post DTO
 */
export interface CreatePostDto {
  title: string;
  content: string;
  excerpt?: string;
  featuredImageUrl?: string;
  publishedAt?: Date;
}

/**
 * Update Post DTO
 */
export interface UpdatePostDto {
  title?: string;
  content?: string;
  excerpt?: string;
  featuredImageUrl?: string;
  status?: EntityStatus;
  publishedAt?: Date;
}

/**
 * Post Query Parameters
 */
export interface PostQueryParams {
  userId?: string;
  status?: EntityStatus;
  search?: string;
  published?: boolean;
  page?: number;
  pageSize?: number;
  sortBy?: 'createdAt' | 'publishedAt' | 'viewCount' | 'commentCount';
  sortOrder?: 'asc' | 'desc';
}
