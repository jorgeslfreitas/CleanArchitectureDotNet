using FluentAssertions;
using Moq;
using Mostruario.Application.DTOs;
using Mostruario.Application.UseCases.Products;
using Mostruario.Domain.Entities;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.Tests.UseCases;

public class CreateProductUseCaseTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateProductUseCase _useCase;

    public CreateProductUseCaseTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _useCase = new CreateProductUseCase(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Execute_ValidDto_ShouldCreateProduct()
    {
        var categoryId = Guid.NewGuid();
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 100.00m,
            Stock = 10,
            CategoryId = categoryId
        };

        var category = new Category("Test Category", "Description");
        
        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _productRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _useCase.ExecuteAsync(dto);

        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.Price.Should().Be(dto.Price);
        
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Execute_CategoryNotFound_ShouldThrowException()
    {
        var categoryId = Guid.NewGuid();
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 100.00m,
            Stock = 10,
            CategoryId = categoryId
        };

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        var act = async () => await _useCase.ExecuteAsync(dto);

        await act.Should().ThrowAsync<Mostruario.Domain.Exceptions.NotFoundException>();
    }
}