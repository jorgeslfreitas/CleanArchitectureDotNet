using Mostruario.Application.Interfaces.Products;
using Mostruario.Domain.Exceptions;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Products;

public class DeleteProductUseCase(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
    : IDeleteProductUseCase
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task ExecuteAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        
        if (product == null)
            throw new NotFoundException("Product", id);

        await _productRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}