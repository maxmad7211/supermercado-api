using Supermercado.Api.Data;
using Supermercado.Api.Infrastructure;
using Supermercado.Api.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Database:Provider elige el motor: "Sqlite" (local y pruebas) o "MySql" (deploy).
// La cadena se resuelve al crear el contexto para que las pruebas puedan sobrescribirla.
static string ConnectionString(IServiceProvider sp) =>
    sp.GetRequiredService<IConfiguration>().GetConnectionString("Supermercado")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:Supermercado");

var useMySql = string.Equals(builder.Configuration["Database:Provider"], "MySql", StringComparison.OrdinalIgnoreCase);

// Sin contraseña en la configuración, quien use la API la manda en el encabezado X-Db-Password.
var passwordFromHeader = useMySql
    && DatabasePassword.IsMissingFrom(builder.Configuration.GetConnectionString("Supermercado") ?? "");

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    DemoGuide.Apply(options, passwordFromHeader);
    if (passwordFromHeader)
    {
        DatabasePassword.AddSecurityScheme(options);
    }
});
builder.Services.AddHttpContextAccessor();

if (useMySql)
{
    builder.Services.AddDbContext<SupermercadoDbContext, MySqlSupermercadoDbContext>((sp, options) =>
        options.UseMySQL(DatabasePassword.ApplyTo(ConnectionString(sp), sp)));
}
else
{
    builder.Services.AddDbContext<SupermercadoDbContext, SqliteSupermercadoDbContext>((sp, options) =>
        options.UseSqlite(ConnectionString(sp)));
}

builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductService>();

var app = builder.Build();

if (passwordFromHeader)
{
    // Las migraciones se aplican con la primera petición que trae la contraseña.
    app.UseMiddleware<DatabasePasswordMiddleware>();
}
else
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<SupermercadoDbContext>().Database.Migrate();
}

// Documentación interactiva siempre disponible para que se pueda probar desde el navegador.
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    if (passwordFromHeader)
    {
        options.AddPreferredSecuritySchemes(DatabasePassword.SchemeName)
            .EnablePersistentAuthentication(); // la contraseña se escribe una sola vez
    }
});
app.MapGet("/", () => Results.Redirect("/scalar")).ExcludeFromDescription();

// Health check de Render; no toca la BD porque sin contraseña no hay conexión.
app.MapGet("/health", () => Results.Ok()).ExcludeFromDescription();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
