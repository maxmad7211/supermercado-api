using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Supermercado.Api.Infrastructure;

/// <summary>
/// Guion de prueba para Scalar: cada paso se vuelve un ejemplo precargado en su endpoint
/// (body e id) y una fila de la tabla en la introducción, así solo hay que dar Send.
/// </summary>
public static class DemoGuide
{
    private sealed record Step(string Method, string Path, string Title, int Expected, object? Body = null, int? Id = null);

    private static readonly Step[] Steps =
    [
        new("POST", "api/demo/reset", "Reiniciar datos (base vacía, ids desde 1)", 204),
        new("POST", "api/categories", "Crear categoría con descripción", 201, new { name = "Lácteos", description = "Leche, quesos y yogurt" }),
        new("POST", "api/categories", "Crear categoría sin descripción", 201, new { name = "Frutas" }),
        new("POST", "api/categories", "Nombre repetido ignorando mayúsculas", 409, new { name = "LÁCTEOS" }),
        new("POST", "api/categories", "Categoría sin nombre", 400, new { description = "Sin nombre" }),
        new("PUT", "api/categories/{id}", "Editar categoría", 200, new { name = "Frutas y verduras", description = "Productos frescos" }, 2),
        new("PUT", "api/categories/{id}", "Editar con el nombre de otra categoría", 409, new { name = "lácteos" }, 2),
        new("PUT", "api/categories/{id}", "Editar sin nombre", 400, new { description = "Sin nombre" }, 2),
        new("GET", "api/categories", "Ver categorías (INDEX)", 200),
        new("POST", "api/products", "Crear producto", 201, new { name = "Leche entera", description = "1 litro", categoryId = 1 }),
        new("POST", "api/products", "Crear producto sin descripción", 201, new { name = "Queso panela", categoryId = 1 }),
new("GET", "api/categories/{id}", "Ver categoría con sus productos (SHOW)", 200, Id: 1),
        new("POST", "api/products", "Producto sin categoría", 400, new { name = "Yogurt" }),
        new("POST", "api/products", "Producto sin nombre", 400, new { description = "Sin nombre", categoryId = 1 }),
        new("POST", "api/products", "Producto en categoría inexistente", 400, new { name = "Pan", categoryId = 999 }),
        new("PUT", "api/products/{id}", "Editar producto cambiándolo de categoría", 200, new { name = "Queso panela", description = "Ahora en Frutas y verduras", categoryId = 2 }, 2),
        new("GET", "api/products", "Ver productos con el nombre de su categoría", 200),
        new("GET", "api/products/{id}", "Ver producto con id y nombre de su categoría", 200, Id: 1),
        new("DELETE", "api/categories/{id}", "Borrar categoría con productos (muestra el mensaje)", 409, Id: 1),
        new("DELETE", "api/products/{id}", "Borrar el último producto de Lácteos", 204, Id: 1),
        new("DELETE", "api/categories/{id}", "Borrar la categoría ya sin productos", 204, Id: 1),
    ];

    public static void Apply(OpenApiOptions options, bool passwordRequired)
    {
        options.AddDocumentTransformer((document, _, _) =>
        {
            document.Info.Title = "Supermercado API";
            document.Info.Description = Introduction(passwordRequired);
            return Task.CompletedTask;
        });

        options.AddOperationTransformer((operation, context, _) =>
        {
            var method = context.Description.HttpMethod;
            var path = context.Description.RelativePath;
            var steps = Steps
                .Select((step, index) => (step, number: index))
                .Where(s => s.step.Method == method && s.step.Path == path)
                .ToList();

            if (steps.Count == 0)
            {
                return Task.CompletedTask;
            }

            operation.Description = string.Join("\n", steps.Select(s =>
                $"- **Paso {s.number}:** {s.step.Title} → `{s.step.Expected}`"));

            var bodies = steps.Where(s => s.step.Body is not null).ToList();
            if (bodies.Count > 0 && operation.RequestBody?.Content is { } content)
            {
                foreach (var mediaType in content.Values.OfType<OpenApiMediaType>())
                {
                    mediaType.Examples = bodies.ToDictionary(
                        s => ExampleName(s.number, s.step),
                        s => (IOpenApiExample)new OpenApiExample
                        {
                            Summary = ExampleName(s.number, s.step),
                            Value = JsonSerializer.SerializeToNode(s.step.Body, JsonSerializerOptions.Web),
                        });
                }
            }

            // Todos los pasos de un mismo endpoint usan el mismo id.
            if (steps.FirstOrDefault(s => s.step.Id is not null).step?.Id is { } id)
            {
                foreach (var parameter in operation.Parameters?.OfType<OpenApiParameter>() ?? [])
                {
                    if (parameter.Name == "id")
                    {
                        parameter.Example = JsonValue.Create(id);
                    }
                }
            }

            return Task.CompletedTask;
        });
    }

    private static string ExampleName(int number, Step step) => $"Paso {number}: {step.Title} ({step.Expected})";

    private static string Introduction(bool passwordRequired)
    {
        var text = new StringBuilder();
        text.AppendLine("API para administrar las categorías y los productos de un supermercado.");
        text.AppendLine();
        text.AppendLine("## Cómo probarla");
        text.AppendLine();
        if (passwordRequired)
        {
            text.AppendLine("1. Escribe la contraseña de la base de datos en **Authentication** (campo `DbPassword`). Solo se pide una vez.");
            text.AppendLine("2. Sigue los pasos en orden. En cada endpoint elige el ejemplo con el número del paso en el menú del **Body** (el id ya viene puesto) y da **Send**.");
        }
        else
        {
            text.AppendLine("Sigue los pasos en orden. En cada endpoint elige el ejemplo con el número del paso en el menú del **Body** (el id ya viene puesto) y da **Send**.");
        }

        text.AppendLine();
        text.AppendLine("| Paso | Endpoint | Qué prueba | Respuesta esperada |");
        text.AppendLine("|---|---|---|---|");
        for (var i = 0; i < Steps.Length; i++)
        {
            var step = Steps[i];
            var path = step.Id is { } id ? step.Path.Replace("{id}", id.ToString()) : step.Path;
            text.AppendLine($"| {i} | `{step.Method} /{path}` | {step.Title} | `{step.Expected}` |");
        }

        text.AppendLine();
        text.AppendLine("El paso 0 deja la base vacía y los ids en 1, para que los ejemplos coincidan con los datos. Puedes repetir el guion las veces que quieras empezando por ahí.");
        return text.ToString();
    }
}
