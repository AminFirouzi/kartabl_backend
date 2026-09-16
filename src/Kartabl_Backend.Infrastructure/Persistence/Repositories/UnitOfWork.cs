using Kartabl_Backend.Application.Common.Interfaces;

namespace Kartabl_Backend.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public IUserRepository Users { get; }
    public IRefreshTokenRepository RefreshTokens { get; }

    public UnitOfWork(
        ApplicationDbContext dbContext,
        IUserRepository users,
        IRefreshTokenRepository refreshTokens)
    {
        _dbContext = dbContext;
        Users = users;
        RefreshTokens = refreshTokens;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}