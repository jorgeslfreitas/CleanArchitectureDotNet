using FluentAssertions;
using Moq;
using Mostruario.Application.DTOs;
using Mostruario.Application.UseCases.Products;
using Mostruario.Domain.Entities;
using Mostruario.Domain.Exceptions;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.Tests.UseCases;

public class UpdateProductUseCaseTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    [Fact]
    public async Task Execute_ProductNotFound_ShouldThrow()
    {
        var dto = new UpdateProductDto { Id = Guid.NewGuid(), Name = "P", Description = "D", Price = 10, Stock = 1, CategoryId = Guid.NewGuid() };

        _productRepositoryMock
            .Setup(x => x.GetByIdWithImagesAsync(dto.Id))
            .ReturnsAsync((Product?)null);

        var useCase = new UpdateProductUseCase(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);

        var act = async () => await useCase.ExecuteAsync(dto);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Execute_CategoryNotFound_ShouldThrow()
    {
        var dto = new UpdateProductDto { Id = Guid.NewGuid(), Name = "P", Description = "D", Price = 10, Stock = 1, CategoryId = Guid.NewGuid() };
        var product = new Product("P", "D", 10, 1, dto.CategoryId);

        _productRepositoryMock
            .Setup(x => x.GetByIdWithImagesAsync(dto.Id))
            .ReturnsAsync(product);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(dto.CategoryId))
            .ReturnsAsync((Category?)null);

        var useCase = new UpdateProductUseCase(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);

        var act = async () => await useCase.ExecuteAsync(dto);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Execute_ValidDto_ShouldUpdateProduct()
    {
        var categoryId = Guid.NewGuid();
        var dto = new UpdateProductDto
        {
            Id = Guid.NewGuid(),
            Name = "Updated",
            Description = "Desc",
            Price = 20,
            Stock = 2,
            CategoryId = categoryId,
            IsActive = true
        };

        var product = new Product("P", "D", 10, 1, categoryId);
        var category = new Category("C", "D");

        _productRepositoryMock
            .Setup(x => x.GetByIdWithImagesAsync(dto.Id))
            .ReturnsAsync(product);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(dto.CategoryId))
            .ReturnsAsync(category);

        var useCase = new UpdateProductUseCase(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);

        var result = await useCase.ExecuteAsync(dto);

        result.Name.Should().Be(dto.Name);
        _productRepositoryMock.Verify(x => x.UpdateAsync(product), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
