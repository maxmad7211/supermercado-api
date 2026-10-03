using System.ComponentModel.DataAnnotations;

namespace Supermercado.Api.Contracts;

public sealed class ProductRequest
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder {1} caracteres")]
    public string? Name { get; init; }

    [StringLength(500, ErrorMessage = "La descripcion no puede exceder {1} caracteres")]
    public string? Description { get; init; }

    [Required(ErrorMessage = "La categoria es obligatoria")]
    public int? CategoryId { get; init; }
}

public sealed record ProductResponse(int Id, string Name, string? Description, CategorySummary Category);

public sealed record ProductSummary(int Id, string Name);
