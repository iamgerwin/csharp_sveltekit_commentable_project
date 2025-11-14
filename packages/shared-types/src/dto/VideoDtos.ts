import { EntityStatus } from '@commentable/shared-enums';

/**
 * Create Video DTO
 */
export interface CreateVideoDto {
  title: string;
  description?: string;
  url: string;
  thumbnailUrl?: string;
  duration: number;
}

/**
 * Update Video DTO
 */
export interface UpdateVideoDto {
  title?: string;
  description?: string;
  thumbnailUrl?: string;
  status?: EntityStatus;
}

/**
 * Video Query Parameters
 */
export interface VideoQueryParams {
  userId?: string;
  status?: EntityStatus;
  search?: string;
  page?: number;
  pageSize?: number;
  sortBy?: 'createdAt' | 'viewCount' | 'commentCount';
  sortOrder?: 'asc' | 'desc';
}
