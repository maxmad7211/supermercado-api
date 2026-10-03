using Supermercado.Api.Contracts;
using Supermercado.Api.Data;
using Supermercado.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Supermercado.Api.Services;

public class CategoryService(SupermercadoDbContext db)
{
    public const string HasProductsMessage = "Esta categoria no se puede eliminar porque tiene productos asignados";

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken ct) =>
        await db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponse(c.Id, c.Name, c.Description))
            .ToListAsync(ct);

    public Task<CategoryDetailResponse?> GetByIdAsync(int id, CancellationToken ct) =>
        db.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoryDetailResponse(
                c.Id,
                c.Name,
                c.Description,
                c.Products.OrderBy(p => p.Name).Select(p => new ProductSummary(p.Id, p.Name)).ToList()))
            .FirstOrDefaultAsync(ct);

    public async Task<Result<CategoryResponse>> CreateAsync(CategoryRequest request, CancellationToken ct)
    {
        var name = request.Name!;
        if (await NameTakenAsync(name, excludeId: null, ct))
        {
            return DuplicateName(name);
        }

        var category = new Category(name, request.Description);
        db.Categories.Add(category);
        await db.SaveChangesAsync(ct);

        return ToResponse(category);
    }

    public async Task<Result<CategoryResponse>> UpdateAsync(int id, CategoryRequest request, CancellationToken ct)
    {
        var category = await db.Categories.FindAsync([id], ct);
        if (category is null)
        {
            return NotFound(id);
        }

        var name = request.Name!;
        if (await NameTakenAsync(name, excludeId: id, ct))
        {
            return DuplicateName(name);
        }

        category.Update(name, request.Description);
        await db.SaveChangesAsync(ct);

        return ToResponse(category);
    }

    public async Task<Error?> DeleteAsync(int id, CancellationToken ct)
    {
        var category = await db.Categories.FindAsync([id], ct);
        if (category is null)
        {
            return NotFound(id);
        }

        if (await db.Products.AnyAsync(p => p.CategoryId == id, ct))
        {
            return new Error(ErrorType.Conflict, HasProductsMessage);
        }

        db.Categories.Remove(category);
        await db.SaveChangesAsync(ct);
        return null;
    }

    private Task<bool> NameTakenAsync(string name, int? excludeId, CancellationToken ct)
    {
        var normalized = Category.Normalize(name);
        return db.Categories.AnyAsync(c => c.NormalizedName == normalized && c.Id != excludeId, ct);
    }

    private static Error DuplicateName(string name) =>
        new(ErrorType.Conflict, $"Ya existe una categoria con el nombre '{name.Trim()}'");

    private static Error NotFound(int id) =>
        new(ErrorType.NotFound, $"No existe la categoria con id {id}");

    private static CategoryResponse ToResponse(Category c) => new(c.Id, c.Name, c.Description);
}
