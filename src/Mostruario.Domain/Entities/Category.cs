using Mostruario.Domain.Base;
using Mostruario.Domain.Exceptions;

namespace Mostruario.Domain.Entities;

public class Category : BaseEntity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    public Category() { }

    public Category(string name, string description)
    {
        Validate(name, description);
        Name = name;
        Description = description;
        IsActive = true;
    }

    public void Update(string name, string description)
    {
        Validate(name, description);
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void Validate(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleException("Category name is required");

        if (name.Length > 100)
            throw new BusinessRuleException("Category name must be at most 100 characters");

        if (description.Length > 500)
            throw new BusinessRuleException("Category description must be at most 500 characters");
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
}