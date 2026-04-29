using Microsoft.EntityFrameworkCore;
using Mostruario.Domain.Entities;
using Mostruario.Domain.Interfaces;
using Mostruario.Infrastructure.Data;

namespace Mostruario.Infrastructure.Repositories;

internal class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly AppDbContext _context = context;

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => EF.Functions.ILike(u.Email, email));
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => EF.Functions.ILike(u.Email, email));
    }


    public async Task<User> AddAsync(User entity)
    {
        await _context.Users.AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(User entity)
    {
        _context.Users.Update(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
        }
    }
}