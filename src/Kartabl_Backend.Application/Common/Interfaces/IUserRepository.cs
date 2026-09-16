using Kartabl_Backend.Domain.Entities;

namespace Kartabl_Backend.Application.Common.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByNationalCodeWithRoleAsync(string nationalCode, CancellationToken cancellationToken = default);
}