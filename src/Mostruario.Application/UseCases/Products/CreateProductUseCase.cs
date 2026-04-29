using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Products;
using Mostruario.Application.Mappings;
using Mostruario.Domain.Exceptions;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Products;

public class CreateProductUseCase(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork)
    : ICreateProductUseCase
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ProductDto> ExecuteAsync(CreateProductDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        
        if (category == null)
            throw new NotFoundException("Category", dto.CategoryId);

        var product = ProductMapper.ToEntity(dto);
        
        await _productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return ProductMapper.ToDto(product);
    }
}