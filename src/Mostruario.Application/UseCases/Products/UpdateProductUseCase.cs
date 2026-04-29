using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Products;
using Mostruario.Application.Mappings;
using Mostruario.Domain.Exceptions;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Products;

public class UpdateProductUseCase(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork)
    : IUpdateProductUseCase
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ProductDto> ExecuteAsync(UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdWithImagesAsync(dto.Id);
        
        if (product == null)
            throw new NotFoundException("Product", dto.Id);

        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category == null)
            throw new NotFoundException("Category", dto.CategoryId);

        ProductMapper.UpdateEntity(product, dto);
        
        await _productRepository.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return ProductMapper.ToDto(product);
    }
}