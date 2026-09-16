using Kartabl_Backend.Domain.Entities;

namespace Kartabl_Backend.Application.Common.Interfaces;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    // Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
}