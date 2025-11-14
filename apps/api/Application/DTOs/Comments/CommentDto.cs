using CommentableAPI.Application.DTOs.Videos;
using CommentableAPI.Domain.Enums;

namespace CommentableAPI.Application.DTOs.Comments;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public Guid CommentableId { get; set; }
    public CommentableType CommentableType { get; set; }
    public Guid? ParentCommentId { get; set; }
    public int ReplyCount { get; set; }
    public ReactionCountsDto ReactionCounts { get; set; } = new();
    public ReactionType? UserReaction { get; set; }
    public EntityStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<CommentDto>? Replies { get; set; }
}

public class CreateCommentDto
{
    public string Content { get; set; } = string.Empty;
    public Guid CommentableId { get; set; }
    public CommentableType CommentableType { get; set; }
    public Guid? ParentCommentId { get; set; }
}

public class UpdateCommentDto
{
    public string Content { get; set; } = string.Empty;
}
