using Mostruario.Domain.Entities;

namespace Mostruario.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task AddAsync(RefreshToken token);
    Task RevokeAsync(RefreshToken token);
}
