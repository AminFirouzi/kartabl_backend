using System.Reflection;
using Kartabl_Backend.Domain.Entities;
using Kartabl_Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kartabl_Backend.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Default Seed Data matching exact UserRole and Permission enums
        modelBuilder.Entity<Role>().HasData(
            new 
            {
                Id = UserRole.SuperUser, // Key: 1
                Permissions = new List<Permission> 
                { 
                    Permission.DocumentRead, 
                    Permission.DocumentCreate, 
                    Permission.DocumentUpdate, 
                    Permission.DocumentDelete,
                    Permission.DocumentVerify,
                    Permission.DocumentReject,
                    Permission.DocumentApprove,
                    Permission.UserRead,
                    Permission.UserCreate,
                    Permission.UserUpdate,
                    Permission.UserDelete,
                    Permission.RoleRead,
                    Permission.RoleManage
                }
            },
            new 
            {
                Id = UserRole.Expert, // Key: 3
                Permissions = new List<Permission> 
                { 
                    Permission.DocumentRead, 
                    Permission.DocumentVerify,
                    Permission.DocumentReject,
                    Permission.DocumentApprove
                }
            },
            new 
            {
                Id = UserRole.Employee, // Key: 2
                Permissions = new List<Permission> 
                { 
                    Permission.DocumentRead, 
                    Permission.DocumentCreate,
                    Permission.DocumentUpdate
                }
            }
        );
    }
}