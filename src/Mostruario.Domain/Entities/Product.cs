using Mostruario.Domain.Base;
using Mostruario.Domain.Exceptions;

namespace Mostruario.Domain.Entities;

public class Product : BaseEntity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public bool IsActive { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }

    private readonly List<ProductImage> _images = new();
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    public Product() { }

    public Product(string name, string description, decimal price, int stock, Guid categoryId)
    {
        Validate(name, description, price, stock, categoryId);
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        CategoryId = categoryId;
        IsActive = true;
    }

    public void Update(string name, string description, decimal price, int stock, Guid categoryId)
    {
        Validate(name, description, price, stock, categoryId);
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddImage(ProductImage image)
    {
        _images.Add(image);
    }

    public void RemoveImage(ProductImage image)
    {
        _images.Remove(image);
    }

    public void ClearImages()
    {
        _images.Clear();
    }

    private static void Validate(string name, string description, decimal price, int stock, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleException("Product name is required");

        if (name.Length > 200)
            throw new BusinessRuleException("Product name must be at most 200 characters");

        if (description.Length > 1000)
            throw new BusinessRuleException("Product description must be at most 1000 characters");

        if (price <= 0)
            throw new BusinessRuleException("Product price must be greater than zero");

        if (stock < 0)
            throw new BusinessRuleException("Product stock cannot be negative");

        if (categoryId == Guid.Empty)
            throw new BusinessRuleException("Category is required");
    }
}