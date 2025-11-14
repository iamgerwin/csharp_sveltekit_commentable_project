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
public class VideosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public VideosController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<VideoDto>>> GetVideos(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortOrder = "desc")
    {
        var query = _context.Videos
            .Include(v => v.User)
            .Where(v => v.Status == EntityStatus.Active);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(v => v.Title.Contains(search) || v.Description.Contains(search));
        }

        var totalCount = await query.CountAsync();

        // Convert camelCase to PascalCase for property names
        var sortProperty = sortBy switch
        {
            "createdAt" => "CreatedAt",
            "updatedAt" => "UpdatedAt",
            "title" => "Title",
            "viewCount" => "ViewCount",
            _ => "CreatedAt"
        };

        query = sortOrder?.ToLower() == "asc"
            ? query.OrderBy(v => EF.Property<object>(v, sortProperty))
            : query.OrderByDescending(v => EF.Property<object>(v, sortProperty));

        var videosQuery = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Calculate comment counts for all videos in this page
        var videoIds = videosQuery.Select(v => v.Id).ToList();
        var commentCounts = await _context.Comments
            .Where(c => videoIds.Contains(c.CommentableId) && c.CommentableType == CommentableType.Video && c.Status == EntityStatus.Active)
            .GroupBy(c => c.CommentableId)
            .Select(g => new { CommentableId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CommentableId, x => x.Count);

        var videos = videosQuery.Select(v => new VideoDto
        {
            Id = v.Id,
            Title = v.Title,
            Description = v.Description,
            VideoUrl = v.VideoUrl,
            ThumbnailUrl = v.ThumbnailUrl,
            Duration = v.Duration,
            UserId = v.UserId,
            Username = v.User != null ? v.User.Username : "Unknown",
            ViewCount = v.ViewCount,
            CommentCount = commentCounts.TryGetValue(v.Id, out var count) ? count : 0,
            ReactionCounts = new ReactionCountsDto
            {
                Like = 0,
                Love = 0,
                Laugh = 0,
                Wow = 0,
                Sad = 0,
                Angry = 0,
                Total = 0
            },
            UserReaction = null,
            Status = v.Status,
            CreatedAt = v.CreatedAt,
            UpdatedAt = v.UpdatedAt
        }).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return Ok(new PaginatedResponse<VideoDto>
        {
            Data = videos,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VideoDto>> GetVideo(Guid id)
    {
        var video = await _context.Videos
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (video == null)
            return NotFound();

        var commentCount = await _context.Comments
            .CountAsync(c => c.CommentableId == video.Id && c.CommentableType == CommentableType.Video && c.Status == EntityStatus.Active);

        return Ok(new VideoDto
        {
            Id = video.Id,
            Title = video.Title,
            Description = video.Description,
            VideoUrl = video.VideoUrl,
            ThumbnailUrl = video.ThumbnailUrl,
            Duration = video.Duration,
            UserId = video.UserId,
            Username = video.User?.Username ?? "Unknown",
            ViewCount = video.ViewCount,
            CommentCount = commentCount,
            ReactionCounts = new ReactionCountsDto(),
            UserReaction = null,
            Status = video.Status,
            CreatedAt = video.CreatedAt,
            UpdatedAt = video.UpdatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<VideoDto>> CreateVideo([FromBody] CreateVideoDto dto)
    {
        // Get user ID from JWT claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid or missing user authentication" });
        }

        var video = new Video
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            VideoUrl = dto.VideoUrl,
            ThumbnailUrl = dto.ThumbnailUrl,
            Duration = dto.Duration ?? 0,
            UserId = userId,
            ViewCount = 0,
            Status = EntityStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Videos.Add(video);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVideo), new { id = video.Id }, await GetVideoDto(video.Id));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<VideoDto>> UpdateVideo(Guid id, [FromBody] UpdateVideoDto dto)
    {
        var video = await _context.Videos.FindAsync(id);
        if (video == null)
            return NotFound();

        if (dto.Title != null) video.Title = dto.Title;
        if (dto.Description != null) video.Description = dto.Description;
        if (dto.VideoUrl != null) video.VideoUrl = dto.VideoUrl;
        if (dto.ThumbnailUrl != null) video.ThumbnailUrl = dto.ThumbnailUrl;
        if (dto.Duration != null) video.Duration = dto.Duration.Value;

        video.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(await GetVideoDto(id));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVideo(Guid id)
    {
        var video = await _context.Videos.FindAsync(id);
        if (video == null)
            return NotFound();

        video.Status = EntityStatus.Removed;
        video.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<VideoDto> GetVideoDto(Guid id)
    {
        var video = await _context.Videos
            .Include(v => v.User)
            .FirstAsync(v => v.Id == id);

        var commentCount = await _context.Comments
            .CountAsync(c => c.CommentableId == id && c.CommentableType == CommentableType.Video && c.Status == EntityStatus.Active);

        return new VideoDto
        {
            Id = video.Id,
            Title = video.Title,
            Description = video.Description,
            VideoUrl = video.VideoUrl,
            ThumbnailUrl = video.ThumbnailUrl,
            Duration = video.Duration,
            UserId = video.UserId,
            Username = video.User?.Username ?? "Unknown",
            ViewCount = video.ViewCount,
            CommentCount = commentCount,
            ReactionCounts = new ReactionCountsDto(),
            UserReaction = null,
            Status = video.Status,
            CreatedAt = video.CreatedAt,
            UpdatedAt = video.UpdatedAt
        };
    }
}

public class PaginatedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
