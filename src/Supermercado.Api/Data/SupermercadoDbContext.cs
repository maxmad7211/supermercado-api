using Supermercado.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Supermercado.Api.Data;

// Modelo común; cada motor tiene su contexto derivado con sus propias migraciones.
public abstract class SupermercadoDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(category =>
        {
            category.Property(c => c.Name).HasMaxLength(100).IsRequired();
            category.Property(c => c.NormalizedName).HasMaxLength(100).IsRequired();
            category.HasIndex(c => c.NormalizedName).IsUnique();
            category.Property(c => c.Description).HasMaxLength(500);

            // Restrict: la BD tampoco permite borrar una categoría con productos.
            category.HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Product>(product =>
        {
            product.Property(p => p.Name).HasMaxLength(100).IsRequired();
            product.Property(p => p.Description).HasMaxLength(500);
        });
    }
}
