using Supermercado.Api.Contracts;
using Supermercado.Api.Data;
using Supermercado.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Supermercado.Api.Services;

public class ProductService(SupermercadoDbContext db)
{
    public const string CategoryNotFoundMessage = "La categoria especificada no existe";

    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(CancellationToken ct) =>
        await db.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(p => new ProductResponse(p.Id, p.Name, p.Description, new CategorySummary(p.Category.Id, p.Category.Name)))
            .ToListAsync(ct);

    public Task<ProductResponse?> GetByIdAsync(int id, CancellationToken ct) =>
        db.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductResponse(p.Id, p.Name, p.Description, new CategorySummary(p.Category.Id, p.Category.Name)))
            .FirstOrDefaultAsync(ct);

    public async Task<Result<ProductResponse>> CreateAsync(ProductRequest request, CancellationToken ct)
    {
        var category = await db.Categories.FindAsync([request.CategoryId!.Value], ct);
        if (category is null)
        {
            return new Error(ErrorType.Validation, CategoryNotFoundMessage);
        }

        var product = new Product(request.Name!, request.Description, category);
        db.Products.Add(product);
        await db.SaveChangesAsync(ct);

        return ToResponse(product);
    }

    public async Task<Result<ProductResponse>> UpdateAsync(int id, ProductRequest request, CancellationToken ct)
    {
        var product = await db.Products.FindAsync([id], ct);
        if (product is null)
        {
            return NotFound(id);
        }

        var category = await db.Categories.FindAsync([request.CategoryId!.Value], ct);
        if (category is null)
        {
            return new Error(ErrorType.Validation, CategoryNotFoundMessage);
        }

        product.Update(request.Name!, request.Description, category);
        await db.SaveChangesAsync(ct);

        return ToResponse(product);
    }

    public async Task<Error?> DeleteAsync(int id, CancellationToken ct)
    {
        var product = await db.Products.FindAsync([id], ct);
        if (product is null)
        {
            return NotFound(id);
        }

        db.Products.Remove(product);
        await db.SaveChangesAsync(ct);
        return null;
    }

    private static Error NotFound(int id) =>
        new(ErrorType.NotFound, $"No existe el producto con id {id}");

    private static ProductResponse ToResponse(Product p) =>
        new(p.Id, p.Name, p.Description, new CategorySummary(p.Category.Id, p.Category.Name));
}
