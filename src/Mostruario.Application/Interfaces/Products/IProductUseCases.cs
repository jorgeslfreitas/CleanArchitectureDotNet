using Mostruario.Application.DTOs;

namespace Mostruario.Application.Interfaces.Products;

public interface IGetAllProductsUseCase
{
    Task<PagedResult<ProductDto>> ExecuteAsync(
        int page = 1,
        int pageSize = 10,
        string? search = null,
        Guid? categoryId = null,
        bool? isActive = null);
}

public interface IGetProductByIdUseCase
{
    Task<ProductDto> ExecuteAsync(Guid id);
}

public interface ICreateProductUseCase
{
    Task<ProductDto> ExecuteAsync(CreateProductDto dto);
}

public interface IUpdateProductUseCase
{
    Task<ProductDto> ExecuteAsync(UpdateProductDto dto);
}

public interface IDeleteProductUseCase
{
    Task ExecuteAsync(Guid id);
}
