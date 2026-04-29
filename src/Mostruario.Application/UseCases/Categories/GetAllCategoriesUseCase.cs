using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Categories;
using Mostruario.Application.Mappings;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Categories;

public class GetAllCategoriesUseCase(ICategoryRepository categoryRepository)
    : IGetAllCategoriesUseCase
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<PagedResult<CategoryDto>> ExecuteAsync(
        int page = 1, 
        int pageSize = 10, 
        bool? isActive = null)
    {
        var (items, totalCount) = await _categoryRepository.GetAllAsync(page, pageSize, isActive ?? true);

        return new PagedResult<CategoryDto>
        {
            Items = items.Select(CategoryMapper.ToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}