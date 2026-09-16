namespace Kartabl_Backend.Domain.Entities;

public class RefreshToken
{
    public int Id { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    
    public DateTime? RevokedAt { get; private set; }
    public int UserId { get; private set; }

    public User User { get; private set; } = null!;

    public bool IsExpired => ExpiresAt <= DateTime.UtcNow;
    public bool IsActive => !IsRevoked && !IsExpired;

    public static RefreshToken Create(
        string tokenHash,
        DateTime expiresAt,
        int userId)
    {
        return new RefreshToken()
        {
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };
    }
}