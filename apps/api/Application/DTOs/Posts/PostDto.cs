using CommentableAPI.Application.DTOs.Videos;
using CommentableAPI.Domain.Enums;

namespace CommentableAPI.Application.DTOs.Posts;

public class PostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int CommentCount { get; set; }
    public ReactionCountsDto ReactionCounts { get; set; } = new();
    public ReactionType? UserReaction { get; set; }
    public EntityStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreatePostDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class UpdatePostDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
}
