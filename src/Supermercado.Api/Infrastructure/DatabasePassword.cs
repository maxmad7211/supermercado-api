using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using MySql.Data.MySqlClient;
using Supermercado.Api.Data;

namespace Supermercado.Api.Infrastructure;

/// <summary>
/// Permite que la contraseña de MySQL no viva en el repositorio: quien usa la API la manda
/// en el encabezado X-Db-Password (Scalar muestra un campo para capturarla).
/// </summary>
public static class DatabasePassword
{
    public const string HeaderName = "X-Db-Password";
    public const string SchemeName = "DbPassword";

    // La contraseña del encabezado, si viene, reemplaza a la de la cadena de conexión.
    public static string ApplyTo(string connectionString, IServiceProvider sp)
    {
        var password = sp.GetRequiredService<IHttpContextAccessor>().HttpContext?.Request.Headers[HeaderName].ToString();
        if (string.IsNullOrEmpty(password))
        {
            return connectionString;
        }

        return new MySqlConnectionStringBuilder(connectionString) { Password = password }.ConnectionString;
    }

    public static bool IsMissingFrom(string connectionString) =>
        string.IsNullOrEmpty(new MySqlConnectionStringBuilder(connectionString).Password);

    public static void AddSecurityScheme(Microsoft.AspNetCore.OpenApi.OpenApiOptions options) =>
        options.AddDocumentTransformer((document, _, _) =>
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = HeaderName,
                Description = "Contraseña de la base de datos MySQL",
            };
            document.Security = [new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference(SchemeName, document)] = [] }];
            return Task.CompletedTask;
        });
}

/// <summary>
/// Exige la contraseña en las rutas /api, aplica las migraciones la primera vez que llega
/// una contraseña válida y traduce el "Access denied" de MySQL a un 401 legible.
/// </summary>
public class DatabasePasswordMiddleware(RequestDelegate next)
{
    private const int AccessDenied = 1045;

    private static readonly SemaphoreSlim MigrationLock = new(1, 1);
    private static bool _migrated;

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await next(context);
            return;
        }

        if (string.IsNullOrEmpty(context.Request.Headers[DatabasePassword.HeaderName]))
        {
            await Unauthorized(context, "Falta la contraseña de la base de datos. Escríbela en el campo de autenticación de Scalar (encabezado X-Db-Password).");
            return;
        }

        try
        {
            await EnsureMigratedAsync(context);
            await next(context);
        }
        catch (Exception ex) when (IsAccessDenied(ex) && !context.Response.HasStarted)
        {
            await Unauthorized(context, "La contraseña de la base de datos es incorrecta.");
        }
    }

    private static async Task EnsureMigratedAsync(HttpContext context)
    {
        if (_migrated)
        {
            return;
        }

        await MigrationLock.WaitAsync(context.RequestAborted);
        try
        {
            if (!_migrated)
            {
                var db = context.RequestServices.GetRequiredService<SupermercadoDbContext>();
                await db.Database.MigrateAsync(context.RequestAborted);
                _migrated = true;
            }
        }
        finally
        {
            MigrationLock.Release();
        }
    }

    private static bool IsAccessDenied(Exception? ex)
    {
        for (; ex is not null; ex = ex.InnerException)
        {
            if (ex is MySqlException { Number: AccessDenied })
            {
                return true;
            }
        }

        return false;
    }

    private static Task Unauthorized(HttpContext context, string detail) =>
        Results.Problem(detail, statusCode: StatusCodes.Status401Unauthorized).ExecuteAsync(context);
}
