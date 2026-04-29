using Mostruario.Application.Interfaces.Categories;
using Mostruario.Domain.Exceptions;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Categories;

public class DeleteCategoryUseCase(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork)
    : IDeleteCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task ExecuteAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        
        if (category == null)
            throw new NotFoundException("Category", id);

        await _categoryRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}