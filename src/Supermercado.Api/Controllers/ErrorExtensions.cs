using Supermercado.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Supermercado.Api.Controllers;

internal static class ErrorExtensions
{
    public static ObjectResult ToProblem(this ControllerBase controller, Error error) =>
        controller.Problem(
            detail: error.Message,
            statusCode: error.Type switch
            {
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest,
            });
}
