namespace Mostruario.Application.DTOs;

public class UpdateProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public Guid CategoryId { get; set; }
    public bool IsActive { get; set; }
    public List<string>? Images { get; set; }
}