using Microsoft.EntityFrameworkCore;

namespace Supermercado.Api.Data;

public class SqliteSupermercadoDbContext(DbContextOptions<SqliteSupermercadoDbContext> options)
    : SupermercadoDbContext(options)
{
    public override Task ResetIdentityAsync(CancellationToken ct) =>
        Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence WHERE name IN ('Products', 'Categories');", ct);
}
