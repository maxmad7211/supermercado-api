namespace Supermercado.Api.Domain;

public class Category
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;

    // Nombre normalizado para garantizar unicidad sin importar mayúsculas/minúsculas.
    public string NormalizedName { get; private set; } = null!;
    public string? Description { get; private set; }
    public ICollection<Product> Products { get; } = new List<Product>();

    private Category() { }

    public Category(string name, string? description) => Update(name, description);

    public void Update(string name, string? description)
    {
        Name = name.Trim();
        NormalizedName = Normalize(name);
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public static string Normalize(string name) => name.Trim().ToUpperInvariant();
}
