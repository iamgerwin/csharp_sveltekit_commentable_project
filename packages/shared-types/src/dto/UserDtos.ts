import { UserRole, EntityStatus } from '@commentable/shared-enums';
import { User } from '../entities';

/**
 * Create User DTO (Registration)
 */
export interface CreateUserDto {
  email: string;
  username: string;
  password: string;
  firstName: string;
  lastName: string;
}

/**
 * Update User DTO
 */
export interface UpdateUserDto {
  firstName?: string;
  lastName?: string;
  avatarUrl?: string;
}

/**
 * Login DTO
 */
export interface LoginDto {
  email: string;
  password: string;
}

/**
 * Auth Response DTO
 */
export interface AuthResponseDto {
  user: Omit<User, 'deletedAt'>;
  token: string;
  refreshToken?: string;
}

/**
 * User Query Parameters
 */
export interface UserQueryParams {
  role?: UserRole;
  status?: EntityStatus;
  search?: string;
  page?: number;
  pageSize?: number;
}
