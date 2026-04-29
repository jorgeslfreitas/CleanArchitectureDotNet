using Mostruario.Domain.Entities;

namespace Mostruario.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(int page, int pageSize, string? search, Guid? categoryId, bool? isActive);
    Task<Product?> GetByIdWithImagesAsync(Guid id);
}