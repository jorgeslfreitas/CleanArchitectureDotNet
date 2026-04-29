using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Categories;
using Mostruario.Application.Mappings;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Categories;

public class CreateCategoryUseCase(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork)
    : ICreateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<CategoryDto> ExecuteAsync(CreateCategoryDto dto)
    {
        var category = CategoryMapper.ToEntity(dto);
        
        await _categoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return CategoryMapper.ToDto(category);
    }
}