using System.Security.Claims;
using CommentableAPI.Domain.Entities;
using CommentableAPI.Domain.Enums;
using CommentableAPI.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommentableAPI.Presentation.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReactionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReactionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Upsert a reaction (create, update, or remove)
    /// </summary>
    [HttpPost("upsert")]
    public async Task<ActionResult<Reaction?>> UpsertReaction([FromBody] UpsertReactionRequest request)
    {
        // Get user ID from JWT claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        // Validate that reactions are only for comments
        if (request.CommentableType != CommentableType.Comment)
        {
            return BadRequest(new { message = "Reactions can only be added to comments. Please add a comment first, then react to the comment." });
        }

        var commentId = request.CommentableId;

        // Validate that the comment exists
        var commentExists = await _context.Comments.AnyAsync(c => c.Id == commentId);
        if (!commentExists)
        {
            return NotFound(new { message = "Comment not found" });
        }

        // Check if user already has a reaction on this comment
        var existingReaction = await _context.Reactions
            .FirstOrDefaultAsync(r =>
                r.UserId == userId &&
                r.CommentId == commentId);

        if (existingReaction != null)
        {
            // If same reaction type, remove it (toggle off)
            if (existingReaction.ReactionType == request.ReactionType)
            {
                _context.Reactions.Remove(existingReaction);
                await _context.SaveChangesAsync();
                return Ok(null); // Reaction removed
            }
            else
            {
                // Update to new reaction type
                existingReaction.ReactionType = request.ReactionType;
                existingReaction.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return Ok(existingReaction);
            }
        }
        else
        {
            // Create new reaction
            var newReaction = new Reaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CommentId = commentId,
                ReactionType = request.ReactionType,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Reactions.Add(newReaction);
            await _context.SaveChangesAsync();
            return Ok(newReaction);
        }
    }

    /// <summary>
    /// Get reactions for a specific comment
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<Reaction>>> GetReactions([FromQuery] Guid? commentId = null)
    {
        var query = _context.Reactions.AsQueryable();

        if (commentId.HasValue)
        {
            query = query.Where(r => r.CommentId == commentId.Value);
        }

        var reactions = await query.ToListAsync();
        return Ok(reactions);
    }

    /// <summary>
    /// Delete a reaction
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReaction(Guid id)
    {
        var reaction = await _context.Reactions.FindAsync(id);

        if (reaction == null)
        {
            return NotFound();
        }

        _context.Reactions.Remove(reaction);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

/// <summary>
/// Request DTO for upserting a reaction
/// </summary>
public class UpsertReactionRequest
{
    public Guid CommentableId { get; set; }
    public CommentableType CommentableType { get; set; }
    public ReactionType ReactionType { get; set; }
}
