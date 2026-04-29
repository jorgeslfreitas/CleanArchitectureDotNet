using System.Security.Cryptography;
using Mostruario.Application.Interfaces.Auth;
using Mostruario.Infrastructure.Configuration;

namespace Mostruario.Infrastructure.Security;

internal class RefreshTokenGenerator(JwtOptions options) : IRefreshTokenGenerator
{
    private readonly JwtOptions _options = options;

    public (string Token, DateTime ExpiresAt) GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(bytes);
        var expiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationInDays);

        return (token, expiresAt);
    }
}
