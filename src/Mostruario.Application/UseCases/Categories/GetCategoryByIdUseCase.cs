using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Categories;
using Mostruario.Application.Mappings;
using Mostruario.Domain.Exceptions;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Categories;

public class GetCategoryByIdUseCase(ICategoryRepository categoryRepository)
    : IGetCategoryByIdUseCase
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<CategoryDto> ExecuteAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdWithProductsAsync(id);
        
        if (category == null)
            throw new NotFoundException("Category", id);

        return CategoryMapper.ToDto(category);
    }
}