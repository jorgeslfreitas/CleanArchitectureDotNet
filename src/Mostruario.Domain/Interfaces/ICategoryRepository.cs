using Mostruario.Domain.Entities;

namespace Mostruario.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<(IEnumerable<Category> Items, int TotalCount)> GetAllAsync(int page, int pageSize, bool? isActive);
    Task<Category?> GetByIdWithProductsAsync(Guid id);
}