using Mostruario.Domain.Entities;
using CategoryDto = Mostruario.Application.DTOs.CategoryDto;
using CreateCategoryDto = Mostruario.Application.DTOs.CreateCategoryDto;
using UpdateCategoryDto = Mostruario.Application.DTOs.UpdateCategoryDto;

namespace Mostruario.Application.Mappings;

public static class CategoryMapper
{
    public static CategoryDto ToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            ProductCount = category.Products?.Count ?? 0,
            CreatedAt = category.CreatedAt
        };
    }

    public static Category ToEntity(CreateCategoryDto dto)
    {
        return new Category(dto.Name, dto.Description);
    }

    public static void UpdateEntity(Category category, UpdateCategoryDto dto)
    {
        category.Update(dto.Name, dto.Description);

        if (dto.IsActive != category.IsActive)
        {
            if (dto.IsActive)
                category.Activate();
            else
                category.Deactivate();
        }
    }
}