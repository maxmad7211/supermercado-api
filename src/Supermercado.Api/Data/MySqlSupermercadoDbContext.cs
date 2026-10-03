using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Supermercado.Api.Data;

public class MySqlSupermercadoDbContext(DbContextOptions<MySqlSupermercadoDbContext> options)
    : SupermercadoDbContext(options);

// Solo para `dotnet ef migrations add`: genera las migraciones de MySQL sin conectarse a un servidor.
public class MySqlSupermercadoDbContextFactory : IDesignTimeDbContextFactory<MySqlSupermercadoDbContext>
{
    public MySqlSupermercadoDbContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<MySqlSupermercadoDbContext>()
            .UseMySQL("Server=localhost;Database=supermercado")
            .Options);
}
