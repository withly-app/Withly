using System.ComponentModel.DataAnnotations;

namespace Withly.Persistence.Entities;

public class RefreshToken(string token, Guid userId)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    [MaxLength(200)]
    public string Token { get; private set; } = token;
    public DateTime ExpiresAt { get; private set; } = DateTime.UtcNow.AddDays(7);
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; private set; }
    public Guid UserId { get; private set; } = userId;

    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public void Revoke() => RevokedAt = DateTime.UtcNow;
}
