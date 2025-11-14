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
        // For now, use a hardcoded user ID (in production, get from authenticated user)
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Check if user already has a reaction on this comment
        var existingReaction = await _context.Reactions
            .FirstOrDefaultAsync(r =>
                r.UserId == userId &&
                r.CommentId == request.CommentId);

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
                CommentId = request.CommentId,
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
    public Guid CommentId { get; set; }
    public ReactionType ReactionType { get; set; }
}
