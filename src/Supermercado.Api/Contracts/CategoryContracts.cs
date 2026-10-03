using System.ComponentModel.DataAnnotations;

namespace Supermercado.Api.Contracts;

public sealed class CategoryRequest
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder {1} caracteres")]
    public string? Name { get; init; }

    [StringLength(500, ErrorMessage = "La descripcion no puede exceder {1} caracteres")]
    public string? Description { get; init; }
}

public sealed record CategoryResponse(int Id, string Name, string? Description);

public sealed record CategoryDetailResponse(
    int Id,
    string Name,
    string? Description,
    IReadOnlyList<ProductSummary> Products);

public sealed record CategorySummary(int Id, string Name);
