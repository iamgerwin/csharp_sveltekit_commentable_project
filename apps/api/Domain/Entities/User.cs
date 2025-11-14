using CommentableAPI.Domain.Enums;

namespace CommentableAPI.Domain.Entities;

/// <summary>
/// User entity for authentication and authorization
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Username for login
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hashed password
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// User's display name
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// User's avatar URL
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// User role (Guest, User, Moderator, Admin)
    /// </summary>
    public UserRole Role { get; set; } = UserRole.User;

    /// <summary>
    /// Entity status (Active, Deleted, Flagged, Removed)
    /// </summary>
    public EntityStatus Status { get; set; } = EntityStatus.Active;

    /// <summary>
    /// User's bio/description
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// When the user was deleted (soft delete)
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public ICollection<Video> Videos { get; set; } = new List<Video>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
}
