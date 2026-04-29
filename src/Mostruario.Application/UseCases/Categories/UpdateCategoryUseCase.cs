using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Categories;
using Mostruario.Application.Mappings;
using Mostruario.Domain.Exceptions;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Categories;

public class UpdateCategoryUseCase(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork)
    : IUpdateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<CategoryDto> ExecuteAsync(UpdateCategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.Id);
        
        if (category == null)
            throw new NotFoundException("Category", dto.Id);

        CategoryMapper.UpdateEntity(category, dto);
        
        await _categoryRepository.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return CategoryMapper.ToDto(category);
    }
}