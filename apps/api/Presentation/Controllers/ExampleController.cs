using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using CommentableAPI.Domain.Enums;

namespace CommentableAPI.Presentation.Controllers;

/// <summary>
/// Example controller demonstrating Swagger/OpenAPI documentation
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[SwaggerTag("Example endpoints demonstrating API documentation patterns")]
public class ExampleController : ControllerBase
{
    /// <summary>
    /// Get all reaction types available in the system
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/v1/example/reaction-types
    ///
    /// Returns a list of all available reaction types that can be used on comments.
    /// </remarks>
    /// <returns>List of reaction types with their metadata</returns>
    /// <response code="200">Returns the list of reaction types</response>
    [HttpGet("reaction-types")]
    [SwaggerOperation(
        Summary = "Get all reaction types",
        Description = "Retrieves a list of all available reaction types in the system",
        OperationId = "GetReactionTypes",
        Tags = new[] { "Example" }
    )]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<ReactionTypeDto>))]
    [ProducesResponseType(typeof(IEnumerable<ReactionTypeDto>), StatusCodes.Status200OK)]
    public IActionResult GetReactionTypes()
    {
        var reactionTypes = Enum.GetValues<ReactionType>()
            .Select(type => new ReactionTypeDto
            {
                Value = type,
                Name = type.ToString(),
                DisplayName = GetDisplayName(type),
                Emoji = GetEmoji(type)
            });

        return Ok(reactionTypes);
    }

    /// <summary>
    /// Get all user roles with their permissions
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/v1/example/user-roles
    ///
    /// Returns detailed information about each user role including their permissions.
    /// </remarks>
    /// <returns>List of user roles with permissions</returns>
    /// <response code="200">Returns the list of user roles</response>
    [HttpGet("user-roles")]
    [SwaggerOperation(
        Summary = "Get all user roles",
        Description = "Retrieves a list of all user roles with their associated permissions",
        OperationId = "GetUserRoles",
        Tags = new[] { "Example" }
    )]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<UserRoleDto>))]
    [ProducesResponseType(typeof(IEnumerable<UserRoleDto>), StatusCodes.Status200OK)]
    public IActionResult GetUserRoles()
    {
        var userRoles = Enum.GetValues<UserRole>()
            .Select(role => new UserRoleDto
            {
                Value = role,
                Name = role.ToString(),
                Description = GetRoleDescription(role),
                Permissions = GetRolePermissions(role)
            });

        return Ok(userRoles);
    }

    /// <summary>
    /// Get entity status options
    /// </summary>
    /// <remarks>
    /// Returns all possible entity status values used for content moderation.
    /// </remarks>
    /// <returns>List of entity status values</returns>
    /// <response code="200">Returns the list of entity statuses</response>
    [HttpGet("entity-statuses")]
    [SwaggerOperation(
        Summary = "Get entity status options",
        Description = "Retrieves all possible entity status values for content moderation",
        OperationId = "GetEntityStatuses"
    )]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<EntityStatusDto>))]
    public IActionResult GetEntityStatuses()
    {
        var statuses = Enum.GetValues<EntityStatus>()
            .Select(status => new EntityStatusDto
            {
                Value = status,
                Name = status.ToString(),
                Description = GetStatusDescription(status),
                IsVisible = status == EntityStatus.Active || status == EntityStatus.Flagged
            });

        return Ok(statuses);
    }

    #region Helper Methods

    private static string GetDisplayName(ReactionType type) => type switch
    {
        ReactionType.Like => "Like",
        ReactionType.Dislike => "Dislike",
        ReactionType.Love => "Love",
        ReactionType.Clap => "Clap",
        ReactionType.Laugh => "Laugh",
        ReactionType.Sad => "Sad",
        _ => type.ToString()
    };

    private static string GetEmoji(ReactionType type) => type switch
    {
        ReactionType.Like => "👍",
        ReactionType.Dislike => "👎",
        ReactionType.Love => "❤️",
        ReactionType.Clap => "👏",
        ReactionType.Laugh => "😄",
        ReactionType.Sad => "😢",
        _ => "❓"
    };

    private static string GetRoleDescription(UserRole role) => role switch
    {
        UserRole.Guest => "Unauthenticated users with view-only access",
        UserRole.User => "Standard authenticated users who can comment and react",
        UserRole.Moderator => "Users with moderation privileges",
        UserRole.Admin => "Full system access",
        _ => role.ToString()
    };

    private static List<string> GetRolePermissions(UserRole role) => role switch
    {
        UserRole.Guest => new() { "ViewComments" },
        UserRole.User => new() { "ViewComments", "CreateComments", "EditOwnComments", "React", "Report" },
        UserRole.Moderator => new() { "ViewComments", "CreateComments", "ModerateComments", "ReviewReports" },
        UserRole.Admin => new() { "AllPermissions" },
        _ => new List<string>()
    };

    private static string GetStatusDescription(EntityStatus status) => status switch
    {
        EntityStatus.Active => "Normal, visible content",
        EntityStatus.Deleted => "Soft deleted by user",
        EntityStatus.Flagged => "Awaiting moderation review",
        EntityStatus.Removed => "Permanently removed by moderator",
        _ => status.ToString()
    };

    #endregion
}

#region DTOs

/// <summary>
/// Reaction type information
/// </summary>
public class ReactionTypeDto
{
    /// <summary>The reaction type enum value</summary>
    [SwaggerSchema("The enum value of the reaction type")]
    public ReactionType Value { get; set; }

    /// <summary>The name of the reaction type</summary>
    [SwaggerSchema("The name of the reaction type")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Display name for UI</summary>
    [SwaggerSchema("Human-readable display name")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Emoji representation</summary>
    [SwaggerSchema("Emoji character representing the reaction")]
    public string Emoji { get; set; } = string.Empty;
}

/// <summary>
/// User role information
/// </summary>
public class UserRoleDto
{
    /// <summary>The user role enum value</summary>
    public UserRole Value { get; set; }

    /// <summary>The name of the role</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description of the role</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>List of permissions granted to this role</summary>
    public List<string> Permissions { get; set; } = new();
}

/// <summary>
/// Entity status information
/// </summary>
public class EntityStatusDto
{
    /// <summary>The entity status enum value</summary>
    public EntityStatus Value { get; set; }

    /// <summary>The name of the status</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description of what this status means</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Whether entities with this status are visible to users</summary>
    public bool IsVisible { get; set; }
}

#endregion
