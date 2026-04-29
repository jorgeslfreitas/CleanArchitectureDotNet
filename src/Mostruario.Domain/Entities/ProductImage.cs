using Mostruario.Domain.Base;
using Mostruario.Domain.Exceptions;

namespace Mostruario.Domain.Entities;

public class ProductImage : BaseEntity<Guid>
{
    public string Url { get; private set; } = string.Empty;
    public bool IsMain { get; private set; }
    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }

    public ProductImage() { }

    public ProductImage(string url, bool isMain = false)
    {
        Validate(url);
        Url = url;
        IsMain = isMain;
    }

    public void SetAsMain()
    {
        IsMain = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveAsMain()
    {
        IsMain = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateUrl(string url)
    {
        Validate(url);
        Url = url;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void Validate(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new BusinessRuleException("Image URL is required");

        if (url.Length > 500)
            throw new BusinessRuleException("Image URL must be at most 500 characters");
    }
}