using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Products;
using Mostruario.Application.Mappings;
using Mostruario.Domain.Exceptions;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Products;

public class GetProductByIdUseCase(IProductRepository productRepository)
    : IGetProductByIdUseCase
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ProductDto> ExecuteAsync(Guid id)
    {
        var product = await _productRepository.GetByIdWithImagesAsync(id);
        
        if (product == null)
            throw new NotFoundException("Product", id);

        return ProductMapper.ToDto(product);
    }
}