using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supermercado.Api.Data;

namespace Supermercado.Api.Controllers;

[ApiController]
[Route("api/demo")]
public class DemoController(SupermercadoDbContext db) : ControllerBase
{
    /// <summary>
    /// Borra todos los productos y categorías y reinicia los ids en 1,
    /// para que los ejemplos precargados en Scalar coincidan con los datos.
    /// </summary>
    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Reset(CancellationToken ct)
    {
        await db.Products.ExecuteDeleteAsync(ct);
        await db.Categories.ExecuteDeleteAsync(ct);
        await db.ResetIdentityAsync(ct);
        return NoContent();
    }
}
