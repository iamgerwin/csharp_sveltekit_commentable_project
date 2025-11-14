using CommentableAPI.Application.DTOs.Comments;
using CommentableAPI.Application.DTOs.Videos;
using CommentableAPI.Domain.Entities;
using CommentableAPI.Domain.Enums;
using CommentableAPI.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CommentableAPI.Presentation.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CommentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<CommentDto>>> GetComments(
        [FromQuery] Guid? commentableId = null,
        [FromQuery] CommentableType? commentableType = null,
        [FromQuery] Guid? parentCommentId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortOrder = "desc")
    {
        var query = _context.Comments
            .Include(c => c.User)
            .Where(c => c.Status == EntityStatus.Active);

        if (commentableId.HasValue)
        {
            query = query.Where(c => c.CommentableId == commentableId.Value);
        }

        if (commentableType.HasValue)
        {
            query = query.Where(c => c.CommentableType == commentableType.Value);
        }

        // Filter by parent comment ID (null for top-level comments)
        if (parentCommentId.HasValue)
        {
            query = query.Where(c => c.ParentCommentId == parentCommentId.Value);
        }
        else
        {
            query = query.Where(c => c.ParentCommentId == null);
        }

        var totalCount = await query.CountAsync();

        // Convert camelCase to PascalCase for property names
        var sortProperty = sortBy switch
        {
            "createdAt" => "CreatedAt",
            "updatedAt" => "UpdatedAt",
            "replyCount" => "ReplyCount",
            _ => "CreatedAt"
        };

        query = sortOrder?.ToLower() == "asc"
            ? query.OrderBy(c => EF.Property<object>(c, sortProperty))
            : query.OrderByDescending(c => EF.Property<object>(c, sortProperty));

        var comments = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                UserId = c.UserId,
                Username = c.User != null ? c.User.Username : "Unknown",
                CommentableId = c.CommentableId,
                CommentableType = c.CommentableType,
                ParentCommentId = c.ParentCommentId,
                ReplyCount = c.ReplyCount,
                ReactionCounts = new ReactionCountsDto(),
                UserReaction = null,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return Ok(new PaginatedResponse<CommentDto>
        {
            Data = comments,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommentDto>> GetComment(Guid id)
    {
        var comment = await _context.Comments
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment == null)
            return NotFound();

        return Ok(new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            UserId = comment.UserId,
            Username = comment.User?.Username ?? "Unknown",
            CommentableId = comment.CommentableId,
            CommentableType = comment.CommentableType,
            ParentCommentId = comment.ParentCommentId,
            ReplyCount = comment.ReplyCount,
            ReactionCounts = new ReactionCountsDto(),
            UserReaction = null,
            Status = comment.Status,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentDto dto)
    {
        // Get user ID from JWT claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid or missing user authentication" });
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Content = dto.Content,
            UserId = userId,
            CommentableId = dto.CommentableId,
            CommentableType = dto.CommentableType,
            ParentCommentId = dto.ParentCommentId,
            Status = EntityStatus.Active,
            ReplyCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, await GetCommentDto(comment.Id));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CommentDto>> UpdateComment(Guid id, [FromBody] UpdateCommentDto dto)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
            return NotFound();

        comment.Content = dto.Content;
        comment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(await GetCommentDto(id));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
            return NotFound();

        comment.Status = EntityStatus.Removed;
        comment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<CommentDto> GetCommentDto(Guid id)
    {
        var comment = await _context.Comments
            .Include(c => c.User)
            .FirstAsync(c => c.Id == id);

        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            UserId = comment.UserId,
            Username = comment.User?.Username ?? "Unknown",
            CommentableId = comment.CommentableId,
            CommentableType = comment.CommentableType,
            ParentCommentId = comment.ParentCommentId,
            ReplyCount = comment.ReplyCount,
            ReactionCounts = new ReactionCountsDto(),
            UserReaction = null,
            Status = comment.Status,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }
}
