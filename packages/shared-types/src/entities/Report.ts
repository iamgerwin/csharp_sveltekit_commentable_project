import { ReportCategory } from '@commentable/shared-enums';
import { BaseEntity } from './BaseEntity';

/**
 * ReportStatus Enum
 */
export enum ReportStatus {
  Pending = 'Pending',
  Reviewed = 'Reviewed',
  Resolved = 'Resolved',
  Dismissed = 'Dismissed',
}

/**
 * Report Entity
 *
 * Represents a user report of an inappropriate comment
 */
export interface Report extends BaseEntity {
  commentId: string;
  reportedByUserId: string;
  category: ReportCategory;
  description?: string | null;
  status: ReportStatus;
  reviewedByUserId?: string | null;
  reviewedAt?: Date | null;
  reviewNotes?: string | null;
}
