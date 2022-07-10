using StyleHelper.Models.Entities;

namespace StyleHelper.Services;

public interface IJwtTokenService
{
    Task<(bool, User?)> IsTokenEligibleForRefreshAsync(string token, string refreshToken, string deviceId);

    Task<string> GenerateAndSaveRefreshTokenForUserAsync(User user, string deviceId);

    string GenerateJwtTokenForUser(User user);
}
