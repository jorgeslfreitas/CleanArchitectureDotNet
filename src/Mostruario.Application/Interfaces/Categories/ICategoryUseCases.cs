using Mostruario.Application.DTOs;

namespace Mostruario.Application.Interfaces.Categories;

public interface IGetAllCategoriesUseCase
{
    Task<PagedResult<CategoryDto>> ExecuteAsync(
        int page = 1,
        int pageSize = 10,
        bool? isActive = null);
}

public interface IGetCategoryByIdUseCase
{
    Task<CategoryDto> ExecuteAsync(Guid id);
}

public interface ICreateCategoryUseCase
{
    Task<CategoryDto> ExecuteAsync(CreateCategoryDto dto);
}

public interface IUpdateCategoryUseCase
{
    Task<CategoryDto> ExecuteAsync(UpdateCategoryDto dto);
}

public interface IDeleteCategoryUseCase
{
    Task ExecuteAsync(Guid id);
}
