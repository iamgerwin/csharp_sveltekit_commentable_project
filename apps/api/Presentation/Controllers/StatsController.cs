using CommentableAPI.Application.DTOs.Stats;
using CommentableAPI.Domain.Enums;
using CommentableAPI.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CommentableAPI.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StatsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    // [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
    public async Task<ActionResult<StatsDto>> GetStats()
    {
        // Get user ID from JWT claims if authenticated
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid? userId = null;
        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedUserId))
        {
            userId = parsedUserId;
        }

        // Execute all count queries in parallel for better performance
        var totalVideosTask = _context.Videos.CountAsync(v => v.Status == EntityStatus.Active);
        var totalPostsTask = _context.Posts.CountAsync(p => p.Status == EntityStatus.Active);
        var totalCommentsTask = _context.Comments.CountAsync(c => c.Status == EntityStatus.Active);
        var totalReactionsTask = _context.Reactions.CountAsync();

        // User-specific stats (only if authenticated)
        Task<int> myVideosTask = Task.FromResult(0);
        Task<int> myPostsTask = Task.FromResult(0);
        Task<int> myCommentsTask = Task.FromResult(0);

        if (userId.HasValue)
        {
            myVideosTask = _context.Videos.CountAsync(v => v.UserId == userId.Value && v.Status == EntityStatus.Active);
            myPostsTask = _context.Posts.CountAsync(p => p.UserId == userId.Value && p.Status == EntityStatus.Active);
            myCommentsTask = _context.Comments.CountAsync(c => c.UserId == userId.Value && c.Status == EntityStatus.Active);
        }

        // Placeholder for future moderation features
        var pendingReportsTask = Task.FromResult(0);
        var flaggedContentTask = Task.FromResult(0);

        // Wait for all queries to complete
        await Task.WhenAll(
            totalVideosTask,
            totalPostsTask,
            totalCommentsTask,
            totalReactionsTask,
            myVideosTask,
            myPostsTask,
            myCommentsTask,
            pendingReportsTask,
            flaggedContentTask
        );

        var stats = new StatsDto
        {
            TotalVideos = await totalVideosTask,
            TotalPosts = await totalPostsTask,
            TotalComments = await totalCommentsTask,
            TotalReactions = await totalReactionsTask,
            MyVideos = await myVideosTask,
            MyPosts = await myPostsTask,
            MyComments = await myCommentsTask,
            PendingReports = await pendingReportsTask,
            FlaggedContent = await flaggedContentTask
        };

        return Ok(stats);
    }
}
