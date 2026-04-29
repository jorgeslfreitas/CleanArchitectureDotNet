using Mostruario.Domain.Base;
using Mostruario.Domain.Exceptions;

namespace Mostruario.Domain.Entities;

public class RefreshToken : BaseEntity<Guid>
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;

    public RefreshToken() { }

    public RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
            throw new BusinessRuleException("UserId is required");
        if (string.IsNullOrWhiteSpace(token))
            throw new BusinessRuleException("Refresh token is required");

        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    public void Revoke()
    {
        if (IsRevoked)
            return;

        RevokedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
