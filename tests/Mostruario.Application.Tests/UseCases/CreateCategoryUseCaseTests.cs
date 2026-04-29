using FluentAssertions;
using Moq;
using Mostruario.Application.DTOs;
using Mostruario.Application.UseCases.Categories;
using Mostruario.Domain.Entities;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.Tests.UseCases;

public class CreateCategoryUseCaseTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    [Fact]
    public async Task Execute_ValidDto_ShouldCreateCategory()
    {
        var dto = new CreateCategoryDto
        {
            Name = "Category",
            Description = "Description"
        };

        _categoryRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync((Category c) => c);

        var useCase = new CreateCategoryUseCase(
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);

        var result = await useCase.ExecuteAsync(dto);

        result.Name.Should().Be(dto.Name);
        _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Category>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
