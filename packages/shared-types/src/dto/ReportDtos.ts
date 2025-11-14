import { ReportCategory } from '@commentable/shared-enums';
import { ReportStatus } from '../entities/Report';

/**
 * Create Report DTO
 */
export interface CreateReportDto {
  commentId: string;
  category: ReportCategory;
  description?: string;
}

/**
 * Update Report DTO (for moderators)
 */
export interface UpdateReportDto {
  status: ReportStatus;
  reviewNotes?: string;
}

/**
 * Report Query Parameters
 */
export interface ReportQueryParams {
  status?: ReportStatus;
  category?: ReportCategory;
  commentId?: string;
  reportedByUserId?: string;
  reviewedByUserId?: string;
  page?: number;
  pageSize?: number;
  sortBy?: 'createdAt' | 'reviewedAt';
  sortOrder?: 'asc' | 'desc';
}
