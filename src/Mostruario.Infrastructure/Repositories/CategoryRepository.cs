using Microsoft.EntityFrameworkCore;
using Mostruario.Domain.Entities;
using Mostruario.Domain.Interfaces;
using Mostruario.Infrastructure.Data;

namespace Mostruario.Infrastructure.Repositories;

internal class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category?> GetByIdWithProductsAsync(Guid id)
    {
        return await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);
    }


    public async Task<(IEnumerable<Category> Items, int TotalCount)> GetAllAsync(
        int page, 
        int pageSize, 
        bool? isActive)
    {
        var query = _context.Categories.AsQueryable();

        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Category> AddAsync(Category entity)
    {
        await _context.Categories.AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(Category entity)
    {
        _context.Categories.Update(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await GetByIdAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
        }
    }
}