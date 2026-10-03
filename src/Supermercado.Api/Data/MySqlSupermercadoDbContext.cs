using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Supermercado.Api.Data;

public class MySqlSupermercadoDbContext(DbContextOptions<MySqlSupermercadoDbContext> options)
    : SupermercadoDbContext(options)
{
    public override async Task ResetIdentityAsync(CancellationToken ct)
    {
        await Database.ExecuteSqlRawAsync("ALTER TABLE `Products` AUTO_INCREMENT = 1;", ct);
        await Database.ExecuteSqlRawAsync("ALTER TABLE `Categories` AUTO_INCREMENT = 1;", ct);
    }
}

// Solo para `dotnet ef migrations add`: genera las migraciones de MySQL sin conectarse a un servidor.
public class MySqlSupermercadoDbContextFactory : IDesignTimeDbContextFactory<MySqlSupermercadoDbContext>
{
    public MySqlSupermercadoDbContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<MySqlSupermercadoDbContext>()
            .UseMySQL("Server=localhost;Database=supermercado")
            .Options);
}
