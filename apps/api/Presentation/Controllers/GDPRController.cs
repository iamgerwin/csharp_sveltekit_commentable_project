using CommentableAPI.Application.DTOs.GDPR;
using CommentableAPI.Domain.Enums;
using CommentableAPI.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CommentableAPI.Presentation.Controllers;

/// <summary>
/// GDPR Compliance Controller
/// Implements Data Subject Rights (Articles 15-21)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GDPRController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GDPRController> _logger;

    public GDPRController(ApplicationDbContext context, ILogger<GDPRController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// GDPR Article 15 - Right to Access
    /// Export all user data in machine-readable format (JSON)
    /// </summary>
    [HttpGet("export")]
    [ProducesResponseType(typeof(UserDataExportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDataExportDto>> ExportUserData()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid or missing user authentication" });
        }

        // Log GDPR data access request (audit requirement)
        _logger.LogInformation("GDPR Data Export Request - User: {UserId} at {Timestamp}",
            userId, DateTime.UtcNow);

        // Fetch user profile
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(new { error = "User not found" });
        }

        // Fetch all user data
        var videos = await _context.Videos
            .Where(v => v.UserId == userId)
            .Select(v => new VideoExportData
            {
                Id = v.Id,
                Title = v.Title,
                Description = v.Description,
                VideoUrl = v.VideoUrl,
                ThumbnailUrl = v.ThumbnailUrl,
                Duration = v.Duration,
                ViewCount = v.ViewCount,
                Status = v.Status,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt
            })
            .ToListAsync();

        var posts = await _context.Posts
            .Where(p => p.UserId == userId)
            .Select(p => new PostExportData
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();

        var comments = await _context.Comments
            .Where(c => c.UserId == userId)
            .Select(c => new CommentExportData
            {
                Id = c.Id,
                Content = c.Content,
                CommentableId = c.CommentableId,
                CommentableType = c.CommentableType,
                ParentCommentId = c.ParentCommentId,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync();

        var reactions = await _context.Reactions
            .Where(r => r.UserId == userId)
            .Select(r => new ReactionExportData
            {
                Id = r.Id,
                CommentId = r.CommentId,
                ReactionType = r.ReactionType,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        // Compile export
        var export = new UserDataExportDto
        {
            Profile = new UserProfileData
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            },
            Videos = videos,
            Posts = posts,
            Comments = comments,
            Reactions = reactions,
            Metadata = new ExportMetadata
            {
                ExportedAt = DateTime.UtcNow,
                TotalVideos = videos.Count,
                TotalPosts = posts.Count,
                TotalComments = comments.Count,
                TotalReactions = reactions.Count
            }
        };

        return Ok(export);
    }

    /// <summary>
    /// GDPR Article 17 - Right to Erasure ("Right to be Forgotten")
    /// Delete user account and all associated data
    /// </summary>
    [HttpDelete("delete-account")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid or missing user authentication" });
        }

        // Verify confirmation
        if (request.Confirmation != "DELETE")
        {
            return BadRequest(new { error = "Invalid confirmation. Type 'DELETE' to confirm account deletion." });
        }

        // Log GDPR erasure request (critical audit requirement)
        _logger.LogWarning("GDPR Account Deletion Request - User: {UserId} at {Timestamp}",
            userId, DateTime.UtcNow);

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(new { error = "User not found" });
        }

        // Soft delete: Mark all content as Removed instead of hard delete
        // This preserves referential integrity and audit trail
        var videos = await _context.Videos.Where(v => v.UserId == userId).ToListAsync();
        foreach (var video in videos)
        {
            video.Status = EntityStatus.Removed;
            video.UpdatedAt = DateTime.UtcNow;
        }

        var posts = await _context.Posts.Where(p => p.UserId == userId).ToListAsync();
        foreach (var post in posts)
        {
            post.Status = EntityStatus.Removed;
            post.UpdatedAt = DateTime.UtcNow;
        }

        var comments = await _context.Comments.Where(c => c.UserId == userId).ToListAsync();
        foreach (var comment in comments)
        {
            comment.Status = EntityStatus.Removed;
            comment.UpdatedAt = DateTime.UtcNow;
        }

        // Hard delete reactions (no PII, can be fully removed)
        var reactions = await _context.Reactions.Where(r => r.UserId == userId).ToListAsync();
        _context.Reactions.RemoveRange(reactions);

        // Anonymize user account (GDPR compliant)
        user.Username = $"deleted_user_{userId.ToString()[..8]}";
        user.Email = $"deleted_{userId.ToString()[..8]}@deleted.local";
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("GDPR Account Deletion Completed - User: {UserId}", userId);

        return NoContent();
    }
}

public class DeleteAccountRequest
{
    public string Confirmation { get; set; } = string.Empty;
}
