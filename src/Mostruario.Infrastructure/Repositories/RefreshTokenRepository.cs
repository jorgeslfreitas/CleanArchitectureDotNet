using Microsoft.EntityFrameworkCore;
using Mostruario.Domain.Entities;
using Mostruario.Domain.Interfaces;
using Mostruario.Infrastructure.Data;

namespace Mostruario.Infrastructure.Repositories;

internal class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    private readonly AppDbContext _context = context;

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == token);
    }

    public async Task AddAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
    }

    public Task RevokeAsync(RefreshToken token)
    {
        token.Revoke();
        _context.RefreshTokens.Update(token);
        return Task.CompletedTask;
    }
}
