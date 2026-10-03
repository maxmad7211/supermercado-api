using Microsoft.EntityFrameworkCore;

namespace Supermercado.Api.Data;

public class SqliteSupermercadoDbContext(DbContextOptions<SqliteSupermercadoDbContext> options)
    : SupermercadoDbContext(options);
