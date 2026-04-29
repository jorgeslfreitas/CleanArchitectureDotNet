using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Products;
using Mostruario.Application.Mappings;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Products;

public class GetAllProductsUseCase(IProductRepository productRepository)
    : IGetAllProductsUseCase
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<PagedResult<ProductDto>> ExecuteAsync(
        int page = 1, 
        int pageSize = 10, 
        string? search = null, 
        Guid? categoryId = null,
        bool? isActive = null)
    {
        var (items, totalCount) = await _productRepository.GetAllAsync(
            page, pageSize, search, categoryId, isActive ?? true);

        return new PagedResult<ProductDto>
        {
            Items = items.Select(ProductMapper.ToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}