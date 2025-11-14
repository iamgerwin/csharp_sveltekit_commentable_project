import { UserRole, EntityStatus } from '@commentable/shared-enums';
import { BaseEntity } from './BaseEntity';

/**
 * User Entity
 *
 * Represents a user in the system
 */
export interface User extends BaseEntity {
  email: string;
  username: string;
  firstName: string;
  lastName: string;
  avatarUrl?: string | null;
  role: UserRole;
  status: EntityStatus;
  emailVerified: boolean;
  lastLoginAt?: Date | null;
}

/**
 * User profile view (public-facing)
 */
export interface UserProfile {
  id: string;
  username: string;
  firstName: string;
  lastName: string;
  avatarUrl?: string | null;
  createdAt: Date;
}
