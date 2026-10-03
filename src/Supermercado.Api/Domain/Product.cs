namespace Supermercado.Api.Domain;

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public int CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    private Product() { }

    public Product(string name, string? description, Category category) => Update(name, description, category);

    public void Update(string name, string? description, Category category)
    {
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Category = category;
        CategoryId = category.Id;
    }
}
