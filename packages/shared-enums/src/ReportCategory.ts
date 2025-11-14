/**
 * ReportCategory Enum
 *
 * Defines the categories for reporting inappropriate comments.
 * Helps moderators quickly categorize and prioritize reports.
 *
 * Usage Example:
 * - Spam: Unsolicited or repetitive content
 * - Harassment: Bullying or targeted harassment
 * - HateSpeech: Discriminatory or hateful content
 * - Violence: Threats or promotion of violence
 * - SexualContent: Inappropriate sexual content
 * - Misinformation: False or misleading information
 * - Other: Other violations not covered by specific categories
 *
 * C# Equivalent: ReportCategory enum in API.Domain.Enums
 */
export enum ReportCategory {
  Spam = 'Spam',
  Harassment = 'Harassment',
  HateSpeech = 'HateSpeech',
  Violence = 'Violence',
  SexualContent = 'SexualContent',
  Misinformation = 'Misinformation',
  Other = 'Other',
}

/**
 * Report category metadata for UI rendering
 */
export interface ReportCategoryMetadata {
  category: ReportCategory;
  label: string;
  description: string;
  severity: 'low' | 'medium' | 'high' | 'critical';
}

/**
 * Get metadata for all report categories
 */
export function getReportCategoryMetadata(): Record<ReportCategory, ReportCategoryMetadata> {
  return {
    [ReportCategory.Spam]: {
      category: ReportCategory.Spam,
      label: 'Spam',
      description: 'Unsolicited, repetitive, or promotional content',
      severity: 'low',
    },
    [ReportCategory.Harassment]: {
      category: ReportCategory.Harassment,
      label: 'Harassment',
      description: 'Bullying, intimidation, or targeted harassment',
      severity: 'high',
    },
    [ReportCategory.HateSpeech]: {
      category: ReportCategory.HateSpeech,
      label: 'Hate Speech',
      description: 'Discriminatory or hateful content targeting individuals or groups',
      severity: 'critical',
    },
    [ReportCategory.Violence]: {
      category: ReportCategory.Violence,
      label: 'Violence',
      description: 'Threats, promotion of violence, or graphic violent content',
      severity: 'critical',
    },
    [ReportCategory.SexualContent]: {
      category: ReportCategory.SexualContent,
      label: 'Sexual Content',
      description: 'Inappropriate sexual or adult content',
      severity: 'high',
    },
    [ReportCategory.Misinformation]: {
      category: ReportCategory.Misinformation,
      label: 'Misinformation',
      description: 'False, misleading, or unverified information',
      severity: 'medium',
    },
    [ReportCategory.Other]: {
      category: ReportCategory.Other,
      label: 'Other',
      description: 'Other policy violations not covered by specific categories',
      severity: 'medium',
    },
  };
}

/**
 * Helper function to validate if a string is a valid ReportCategory
 */
export function isValidReportCategory(value: string): value is ReportCategory {
  return Object.values(ReportCategory).includes(value as ReportCategory);
}

/**
 * Get all report categories as an array
 */
export function getAllReportCategories(): ReportCategory[] {
  return Object.values(ReportCategory);
}
