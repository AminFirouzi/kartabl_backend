namespace Kartabl_Backend.Domain.Entities;

public class User
{
    // fields
    public int Id { get; private set; }
    public string NationalCode { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public int RoleId { get; private set; }
    
    
    // navigation properties
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    public Role Role { get; private set; }
    
    public User(){}
    
    // factory method   
    public static User Create(
        string nationalCode,
        string firstName,
        string lastName,
        string passwordHash,
        int roleId,
        bool isActive = true,
        bool isDeleted = false
    )
    {
        return new User()
        {
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            RoleId = roleId,
            IsActive = isActive,
            IsDeleted = isDeleted,
            CreatedAt = DateTime.UtcNow
        };
    }
}