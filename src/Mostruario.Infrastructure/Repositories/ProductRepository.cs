using Microsoft.EntityFrameworkCore;
using Mostruario.Domain.Entities;
using Mostruario.Domain.Interfaces;
using Mostruario.Infrastructure.Data;

namespace Mostruario.Infrastructure.Repositories;

internal class ProductRepository(AppDbContext context) : IProductRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetByIdWithImagesAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);
    }


    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(
        int page, 
        int pageSize, 
        string? search, 
        Guid? categoryId, 
        bool? isActive)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchPattern = $"%{search}%";
            query = query.Where(p =>
                EF.Functions.ILike(p.Name, searchPattern) ||
                EF.Functions.ILike(p.Description, searchPattern));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Product> AddAsync(Product entity)
    {
        await _context.Products.AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(Product entity)
    {
        _context.Products.Update(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await GetByIdAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
        }
    }
}