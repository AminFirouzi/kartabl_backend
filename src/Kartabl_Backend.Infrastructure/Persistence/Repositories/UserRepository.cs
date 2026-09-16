using Kartabl_Backend.Application.Common.Interfaces;
using Kartabl_Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kartabl_Backend.Infrastructure.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<User?> GetByNationalCodeWithRoleAsync(string nationalCode, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.NationalCode == nationalCode && !u.IsDeleted, cancellationToken);
    }
}