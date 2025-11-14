using CommentableAPI.Application.DTOs.Videos;
using CommentableAPI.Application.DTOs.Posts;
using CommentableAPI.Domain.Enums;

namespace CommentableAPI.Application.DTOs.GDPR;

/// <summary>
/// GDPR Article 15 - Right to Access
/// Complete export of all user data in machine-readable format
/// </summary>
public class UserDataExportDto
{
    public UserProfileData Profile { get; set; } = new();
    public List<VideoExportData> Videos { get; set; } = new();
    public List<PostExportData> Posts { get; set; } = new();
    public List<CommentExportData> Comments { get; set; } = new();
    public List<ReactionExportData> Reactions { get; set; } = new();
    public ExportMetadata Metadata { get; set; } = new();
}

public class UserProfileData
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class VideoExportData
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public int Duration { get; set; }
    public int ViewCount { get; set; }
    public EntityStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PostExportData
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public EntityStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CommentExportData
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid CommentableId { get; set; }
    public CommentableType CommentableType { get; set; }
    public Guid? ParentCommentId { get; set; }
    public EntityStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ReactionExportData
{
    public Guid Id { get; set; }
    public Guid CommentId { get; set; }
    public ReactionType ReactionType { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ExportMetadata
{
    public DateTime ExportedAt { get; set; } = DateTime.UtcNow;
    public string ExportFormat { get; set; } = "JSON";
    public string GdprArticle { get; set; } = "Article 15 - Right to Access";
    public int TotalVideos { get; set; }
    public int TotalPosts { get; set; }
    public int TotalComments { get; set; }
    public int TotalReactions { get; set; }
}
