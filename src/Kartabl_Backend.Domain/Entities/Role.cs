using Kartabl_Backend.Domain.Enums;

namespace Kartabl_Backend.Domain.Entities;

public class Role
{
    private Role() { }

    public UserRole Id { get; private set; }

    public IReadOnlyCollection<Permission> Permissions { get; private set; } = new List<Permission>();

    public ICollection<User> Users { get; private set; } = new List<User>();

    public static Role Create(UserRole role, IEnumerable<Permission> permissions)
    {
        return new Role
        {
            Id = role,
            Permissions = permissions.Distinct().ToList().AsReadOnly()
        };
    }

    // Encapsulated method to update permissions
    public void UpdatePermissions(IEnumerable<Permission> newPermissions)
    {
        Permissions = newPermissions.Distinct().ToList().AsReadOnly();
    }
}