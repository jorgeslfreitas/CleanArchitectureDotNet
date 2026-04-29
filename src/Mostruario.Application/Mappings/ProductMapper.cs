using Mostruario.Domain.Entities;
using ProductDto = Mostruario.Application.DTOs.ProductDto;
using CreateProductDto = Mostruario.Application.DTOs.CreateProductDto;
using UpdateProductDto = Mostruario.Application.DTOs.UpdateProductDto;

namespace Mostruario.Application.Mappings;

public static class ProductMapper
{
    public static ProductDto ToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            Images = product.Images.Select(i => i.Url).ToList(),
            CreatedAt = product.CreatedAt
        };
    }

    public static Product ToEntity(CreateProductDto dto)
    {
        var product = new Product(
            dto.Name,
            dto.Description,
            dto.Price,
            dto.Stock,
            dto.CategoryId
        );

        if (dto.Images != null)
        {
            foreach (var imageUrl in dto.Images)
            {
                product.AddImage(new ProductImage(imageUrl));
            }
        }

        return product;
    }

    public static void UpdateEntity(Product product, UpdateProductDto dto)
    {
        product.Update(dto.Name, dto.Description, dto.Price, dto.Stock, dto.CategoryId);
        
        if (dto.IsActive != product.IsActive)
        {
            if (dto.IsActive)
                product.Activate();
            else
                product.Deactivate();
        }

        if (dto.Images != null)
        {
            product.ClearImages();
            foreach (var imageUrl in dto.Images)
            {
                product.AddImage(new ProductImage(imageUrl));
            }
        }
    }
}