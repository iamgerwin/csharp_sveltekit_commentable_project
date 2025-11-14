using CommentableAPI.Domain.Enums;

namespace CommentableAPI.Application.DTOs.Videos;

public class VideoDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public int? Duration { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int CommentCount { get; set; }
    public ReactionCountsDto ReactionCounts { get; set; } = new();
    public ReactionType? UserReaction { get; set; }
    public EntityStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateVideoDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public int? Duration { get; set; }
}

public class UpdateVideoDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int? Duration { get; set; }
}

public class ReactionCountsDto
{
    public int Like { get; set; }
    public int Love { get; set; }
    public int Laugh { get; set; }
    public int Wow { get; set; }
    public int Sad { get; set; }
    public int Angry { get; set; }
    public int Total { get; set; }
}
