using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Supermercado.Specs.Support;

/// <summary>
/// Estado compartido de un escenario: cliente HTTP, última respuesta e ids por nombre.
/// Reqnroll lo crea por escenario y lo libera al terminar.
/// </summary>
public sealed class ApiContext : IDisposable
{
    public const int MissingId = 999_999;

    private readonly SupermercadoApiFactory _factory = new();

    public ApiContext() => Client = _factory.CreateClient();

    public HttpClient Client { get; }
    public HttpResponseMessage? LastResponse { get; set; }
    public Dictionary<string, int> CategoryIds { get; } = new();
    public Dictionary<string, int> ProductIds { get; } = new();

    public HttpResponseMessage Response =>
        LastResponse ?? throw new InvalidOperationException("Aún no se ha hecho ninguna petición.");

    // ReadAsStringAsync deja el contenido en buffer, así varios pasos pueden leer la misma respuesta.
    public async Task<T> ReadAsync<T>()
    {
        var json = await Response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions.Web)
            ?? throw new InvalidOperationException("La respuesta no tiene cuerpo.");
    }

    public async Task<IReadOnlyList<string>> ReadErrorMessagesAsync()
    {
        var problem = await ReadAsync<ValidationProblemDetails>();
        return [.. problem.Errors.SelectMany(e => e.Value), .. problem.Detail is null ? [] : new[] { problem.Detail }];
    }

    public void Dispose()
    {
        LastResponse?.Dispose();
        Client.Dispose();
        _factory.Dispose();
    }
}
