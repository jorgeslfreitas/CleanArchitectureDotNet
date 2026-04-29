namespace Mostruario.Infrastructure.Configuration;

public class JwtOptions
{
    public string SecretKey { get; init; } = string.Empty;
    public string Issuer { get; init; } = "MostruarioApi";
    public string Audience { get; init; } = "MostruarioApi";
    public int ExpirationInMinutes { get; init; } = 60;
    public int RefreshTokenExpirationInDays { get; init; } = 7;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(SecretKey) || SecretKey.Length < 32)
        {
            throw new InvalidOperationException("JWT SecretKey must be at least 32 characters");
        }

        if (string.IsNullOrWhiteSpace(Issuer))
        {
            throw new InvalidOperationException("JWT Issuer not configured");
        }

        if (string.IsNullOrWhiteSpace(Audience))
        {
            throw new InvalidOperationException("JWT Audience not configured");
        }

        if (RefreshTokenExpirationInDays <= 0)
        {
            throw new InvalidOperationException("Refresh token expiration must be greater than zero");
        }
    }
}
