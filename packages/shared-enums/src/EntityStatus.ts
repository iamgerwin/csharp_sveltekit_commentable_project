/**
 * EntityStatus Enum
 *
 * Defines the status of entities (comments, posts, videos, etc.)
 * Used for soft deletion and content moderation.
 *
 * Status Flow:
 * Active -> (user deletes) -> Deleted
 * Active -> (moderator flags) -> Flagged -> (moderator reviews) -> Active | Removed
 * Active -> (auto-moderation) -> Flagged
 *
 * - Active: Normal, visible entity
 * - Deleted: Soft deleted by user (can be restored within grace period)
 * - Flagged: Awaiting moderation review
 * - Removed: Permanently removed by moderator
 *
 * C# Equivalent: EntityStatus enum in API.Domain.Enums
 */
export enum EntityStatus {
  Active = 'Active',
  Deleted = 'Deleted',
  Flagged = 'Flagged',
  Removed = 'Removed',
}

/**
 * Entity status metadata for UI rendering
 */
export interface StatusMetadata {
  status: EntityStatus;
  label: string;
  description: string;
  isVisible: boolean;
  canBeRestored: boolean;
  color: string;
}

/**
 * Get metadata for all entity statuses
 */
export function getStatusMetadata(): Record<EntityStatus, StatusMetadata> {
  return {
    [EntityStatus.Active]: {
      status: EntityStatus.Active,
      label: 'Active',
      description: 'Normal, visible content',
      isVisible: true,
      canBeRestored: false,
      color: '#10b981', // green
    },
    [EntityStatus.Deleted]: {
      status: EntityStatus.Deleted,
      label: 'Deleted',
      description: 'Soft deleted by user',
      isVisible: false,
      canBeRestored: true,
      color: '#6b7280', // gray
    },
    [EntityStatus.Flagged]: {
      status: EntityStatus.Flagged,
      label: 'Flagged',
      description: 'Awaiting moderation review',
      isVisible: true,
      canBeRestored: false,
      color: '#f59e0b', // yellow
    },
    [EntityStatus.Removed]: {
      status: EntityStatus.Removed,
      label: 'Removed',
      description: 'Permanently removed by moderator',
      isVisible: false,
      canBeRestored: false,
      color: '#ef4444', // red
    },
  };
}

/**
 * Helper function to validate if a string is a valid EntityStatus
 */
export function isValidEntityStatus(value: string): value is EntityStatus {
  return Object.values(EntityStatus).includes(value as EntityStatus);
}

/**
 * Get all entity statuses as an array
 */
export function getAllEntityStatuses(): EntityStatus[] {
  return Object.values(EntityStatus);
}

/**
 * Check if entity with given status should be visible to users
 */
export function isVisible(status: EntityStatus): boolean {
  return getStatusMetadata()[status].isVisible;
}

/**
 * Check if entity with given status can be restored
 */
export function canBeRestored(status: EntityStatus): boolean {
  return getStatusMetadata()[status].canBeRestored;
}
