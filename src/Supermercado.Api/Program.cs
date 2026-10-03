using Supermercado.Api.Data;
using Supermercado.Api.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database:Provider elige el motor: "Sqlite" (local y pruebas) o "MySql" (deploy).
// La cadena se resuelve al crear el contexto para que las pruebas puedan sobrescribirla.
static string ConnectionString(IServiceProvider sp) =>
    sp.GetRequiredService<IConfiguration>().GetConnectionString("Supermercado")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:Supermercado");

var provider = builder.Configuration["Database:Provider"] ?? "Sqlite";
if (provider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<SupermercadoDbContext, MySqlSupermercadoDbContext>((sp, options) =>
        options.UseMySQL(ConnectionString(sp)));
}
else
{
    builder.Services.AddDbContext<SupermercadoDbContext, SqliteSupermercadoDbContext>((sp, options) =>
        options.UseSqlite(ConnectionString(sp)));
}

builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<SupermercadoDbContext>().Database.Migrate();
}

// Documentación interactiva siempre disponible para que se pueda probar desde el navegador.
app.MapOpenApi();
app.MapScalarApiReference();
app.MapGet("/", () => Results.Redirect("/scalar")).ExcludeFromDescription();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
