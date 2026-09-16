using Kartabl_Backend.Domain.Entities;

namespace Kartabl_Backend.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user, IEnumerable<string> permissions);
    string GenerateRefreshToken();
    string HashRefreshToken(string rawToken);
}