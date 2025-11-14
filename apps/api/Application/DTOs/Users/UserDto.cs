using CommentableAPI.Domain.Enums;

namespace CommentableAPI.Application.DTOs.Users;

/// <summary>
/// User data transfer object
/// </summary>
public class UserDto
{
    /// <summary>
    /// User unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Unique username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// User email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User role (Guest, User, Moderator, Admin)
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Timestamp when user was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when user was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
