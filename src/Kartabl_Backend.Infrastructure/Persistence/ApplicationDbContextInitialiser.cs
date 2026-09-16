using Kartabl_Backend.Domain.Entities;
using Kartabl_Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kartabl_Backend.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;

    public ApplicationDbContextInitialiser(
        ApplicationDbContext context,
        ILogger<ApplicationDbContextInitialiser> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while running database migrations.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding default roles and superuser.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // 1. Seed Default Roles with Granular Enum Permissions
        if (!await _context.Roles.AnyAsync())
        {
            var superUserPermissions = Enum.GetValues<Permission>().ToList();

            var expertPermissions = new List<Permission>
            {
                Permission.DocumentRead,
                Permission.DocumentCreate,
                Permission.DocumentUpdate,
                Permission.DocumentVerify,
                Permission.DocumentReject,
                Permission.DocumentApprove,
                Permission.UserRead
            };

            var employeePermissions = new List<Permission>
            {
                Permission.DocumentRead,
                Permission.DocumentCreate,
                Permission.DocumentDelete,
                Permission.DocumentUpdate
            };

            var roles = new List<Role>
            {
                Role.Create(UserRole.SuperUser, superUserPermissions),
                Role.Create(UserRole.Expert, expertPermissions),
                Role.Create(UserRole.Employee, employeePermissions)
            };

            await _context.Roles.AddRangeAsync(roles);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Roles and permissions seeded successfully.");
        }
        else
        {
            _logger.LogInformation("Roles already exist in the database. Skipping role seeding.");
        }

        // 2. Seed Initial SuperUser Account
        if (!await _context.Users.AnyAsync(u => u.RoleId == UserRole.SuperUser))
        {
            // Generates a valid BCrypt hash for "Admin@123"
            var defaultPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123", workFactor: 11);

            var superUser = User.Create(
                nationalCode: "0012345678",
                firstName: "System",
                lastName: "SuperUser",
                passwordHash: defaultPasswordHash,
                roleId: UserRole.SuperUser,
                isActive: true
            );

            await _context.Users.AddAsync(superUser);
            await _context.SaveChangesAsync();
            _logger.LogInformation("SuperUser account seeded successfully.");
        }
        else
        {
            _logger.LogInformation("SuperUser account already exists. Skipping user seeding.");
        }
    }
}