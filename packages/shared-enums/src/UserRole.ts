/**
 * UserRole Enum
 *
 * Defines user roles for access control and permissions.
 *
 * Role Hierarchy (from lowest to highest):
 * - Guest: Unauthenticated users (view only)
 * - User: Standard authenticated users (can comment and react)
 * - Moderator: Can moderate comments and reports
 * - Admin: Full system access
 *
 * C# Equivalent: UserRole enum in API.Domain.Enums
 */
export enum UserRole {
  Guest = 'Guest',
  User = 'User',
  Moderator = 'Moderator',
  Admin = 'Admin',
}

/**
 * User role permissions
 */
export interface RolePermissions {
  role: UserRole;
  canViewComments: boolean;
  canCreateComments: boolean;
  canEditOwnComments: boolean;
  canDeleteOwnComments: boolean;
  canReact: boolean;
  canReport: boolean;
  canModerateComments: boolean;
  canManageUsers: boolean;
}

/**
 * Get permissions for a specific role
 */
export function getRolePermissions(role: UserRole): RolePermissions {
  const permissions: Record<UserRole, RolePermissions> = {
    [UserRole.Guest]: {
      role: UserRole.Guest,
      canViewComments: true,
      canCreateComments: false,
      canEditOwnComments: false,
      canDeleteOwnComments: false,
      canReact: false,
      canReport: false,
      canModerateComments: false,
      canManageUsers: false,
    },
    [UserRole.User]: {
      role: UserRole.User,
      canViewComments: true,
      canCreateComments: true,
      canEditOwnComments: true,
      canDeleteOwnComments: true,
      canReact: true,
      canReport: true,
      canModerateComments: false,
      canManageUsers: false,
    },
    [UserRole.Moderator]: {
      role: UserRole.Moderator,
      canViewComments: true,
      canCreateComments: true,
      canEditOwnComments: true,
      canDeleteOwnComments: true,
      canReact: true,
      canReport: true,
      canModerateComments: true,
      canManageUsers: false,
    },
    [UserRole.Admin]: {
      role: UserRole.Admin,
      canViewComments: true,
      canCreateComments: true,
      canEditOwnComments: true,
      canDeleteOwnComments: true,
      canReact: true,
      canReport: true,
      canModerateComments: true,
      canManageUsers: true,
    },
  };

  return permissions[role];
}

/**
 * Helper function to validate if a string is a valid UserRole
 */
export function isValidUserRole(value: string): value is UserRole {
  return Object.values(UserRole).includes(value as UserRole);
}

/**
 * Get all user roles as an array
 */
export function getAllUserRoles(): UserRole[] {
  return Object.values(UserRole);
}

/**
 * Check if role has specific permission
 */
export function hasPermission(
  role: UserRole,
  permission: keyof Omit<RolePermissions, 'role'>
): boolean {
  return getRolePermissions(role)[permission];
}
