using CommentableAPI.Domain.Entities;

namespace CommentableAPI.Application.Services.Auth;

/// <summary>
/// Service for JWT token generation and validation
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generate access token for authenticated user
    /// </summary>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Generate refresh token for token renewal
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Get token expiration time in seconds
    /// </summary>
    int GetTokenExpirationSeconds();
}
