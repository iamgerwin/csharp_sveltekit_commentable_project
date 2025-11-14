namespace CommentableAPI.Domain.Enums;

/// <summary>
/// Defines user roles for access control and permissions.
///
/// Role Hierarchy (from lowest to highest):
/// - Guest: Unauthenticated users (view only)
/// - User: Standard authenticated users (can comment and react)
/// - Moderator: Can moderate comments and reports
/// - Admin: Full system access
///
/// TypeScript Equivalent: UserRole enum in @commentable/shared-enums
/// </summary>
public enum UserRole
{
    Guest,
    User,
    Moderator,
    Admin
}
