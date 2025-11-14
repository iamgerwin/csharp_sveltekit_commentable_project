using CommentableAPI.Application.DTOs.Posts;
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
public class PostsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PostsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<PostDto>>> GetPosts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortOrder = "desc")
    {
        var query = _context.Posts
            .Include(p => p.User)
            .Where(p => p.Status == EntityStatus.Active);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Title.Contains(search) || p.Content.Contains(search));
        }

        var totalCount = await query.CountAsync();

        // Convert camelCase to PascalCase for property names
        var sortProperty = sortBy switch
        {
            "createdAt" => "CreatedAt",
            "updatedAt" => "UpdatedAt",
            "title" => "Title",
            _ => "CreatedAt"
        };

        query = sortOrder?.ToLower() == "asc"
            ? query.OrderBy(p => EF.Property<object>(p, sortProperty))
            : query.OrderByDescending(p => EF.Property<object>(p, sortProperty));

        var postsQuery = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Calculate comment counts for all posts in this page
        var postIds = postsQuery.Select(p => p.Id).ToList();
        var commentCounts = await _context.Comments
            .Where(c => postIds.Contains(c.CommentableId) && c.CommentableType == CommentableType.Post && c.Status == EntityStatus.Active)
            .GroupBy(c => c.CommentableId)
            .Select(g => new { CommentableId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CommentableId, x => x.Count);

        var posts = postsQuery.Select(p => new PostDto
        {
            Id = p.Id,
            Title = p.Title,
            Content = p.Content,
            UserId = p.UserId,
            Username = p.User != null ? p.User.Username : "Unknown",
            CommentCount = commentCounts.TryGetValue(p.Id, out var count) ? count : 0,
            ReactionCounts = new ReactionCountsDto(),
            UserReaction = null,
            Status = p.Status,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return Ok(new PaginatedResponse<PostDto>
        {
            Data = posts,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PostDto>> GetPost(Guid id)
    {
        var post = await _context.Posts
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            return NotFound();

        var commentCount = await _context.Comments
            .CountAsync(c => c.CommentableId == post.Id && c.CommentableType == CommentableType.Post && c.Status == EntityStatus.Active);

        return Ok(new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            UserId = post.UserId,
            Username = post.User?.Username ?? "Unknown",
            CommentCount = commentCount,
            ReactionCounts = new ReactionCountsDto(),
            UserReaction = null,
            Status = post.Status,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostDto dto)
    {
        // Get user ID from JWT claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid or missing user authentication" });
        }

        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Content = dto.Content,
            
            UserId = userId,
            Status = EntityStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, await GetPostDto(post.Id));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PostDto>> UpdatePost(Guid id, [FromBody] UpdatePostDto dto)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
            return NotFound();

        if (dto.Title != null) post.Title = dto.Title;
        if (dto.Content != null) post.Content = dto.Content;
        

        post.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(await GetPostDto(id));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
            return NotFound();

        post.Status = EntityStatus.Removed;
        post.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<PostDto> GetPostDto(Guid id)
    {
        var post = await _context.Posts
            .Include(p => p.User)
            .FirstAsync(p => p.Id == id);

        var commentCount = await _context.Comments
            .CountAsync(c => c.CommentableId == id && c.CommentableType == CommentableType.Post && c.Status == EntityStatus.Active);

        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            UserId = post.UserId,
            Username = post.User?.Username ?? "Unknown",
            CommentCount = commentCount,
            ReactionCounts = new ReactionCountsDto(),
            UserReaction = null,
            Status = post.Status,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}
