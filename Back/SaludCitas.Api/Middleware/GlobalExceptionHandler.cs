using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SaludCitas.Application.Common;

namespace SaludCitas.Api.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, titulo) = exception switch
        {
            BadRequestException => (StatusCodes.Status400BadRequest, "Solicitud no válida"),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "No autorizado"),
            NotFoundException => (StatusCodes.Status404NotFound, "No encontrado"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflicto"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
        };

        string detalle;
        if (exception is AppException)
        {
            detalle = exception.Message;
        }
        else
        {
            logger.LogError(exception, "Error no controlado");
            detalle = "Ocurrió un error inesperado. Intenta de nuevo más tarde.";
        }

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails { Status = status, Title = titulo, Detail = detalle },
            cancellationToken);

        return true;
    }
}