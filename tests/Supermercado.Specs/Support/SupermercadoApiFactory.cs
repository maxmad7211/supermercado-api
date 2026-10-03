using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;

namespace Supermercado.Specs.Support;

/// <summary>
/// Levanta la API en memoria con una base SQLite aislada por escenario.
/// </summary>
public sealed class SupermercadoApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString =
        $"Data Source=supermercado-specs-{Guid.NewGuid():N};Mode=Memory;Cache=Shared";

    // Una BD SQLite en memoria vive mientras haya al menos una conexión abierta.
    private readonly SqliteConnection _keepAlive;

    public SupermercadoApiFactory()
    {
        _keepAlive = new SqliteConnection(_connectionString);
        _keepAlive.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:Supermercado", _connectionString);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _keepAlive.Dispose();
        }
    }
}
