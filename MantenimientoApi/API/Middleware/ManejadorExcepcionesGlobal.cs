using System.Diagnostics;
using Abstracciones.Excepciones;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Middleware;

/// <summary>
/// Manejador global de excepciones (IExceptionHandler) que intercepta errores y
/// los traduce al formato estándar RFC 7807 (application/problem+json).
/// </summary>
public class ManejadorExcepcionesGlobal : IExceptionHandler
{
    private readonly ILogger<ManejadorExcepcionesGlobal> _logger;

    public ManejadorExcepcionesGlobal(ILogger<ManejadorExcepcionesGlobal> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Ocurrió una excepción capturada por el manejador global: {Mensaje}", exception.Message);

        var (statusCode, title, detail) = exception switch
        {
            NoEncontradoException ex => (ex.CodigoEstado, ex.Titulo, ex.Message),
            ReglaNegocioException ex => (ex.CodigoEstado, ex.Titulo, ex.Message),
            ConflictoException ex => (ex.CodigoEstado, ex.Titulo, ex.Message),
            NoAutorizadoException ex => (ex.CodigoEstado, ex.Titulo, ex.Message),
            DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, 
                "Conflicto de concurrencia optimista", 
                "El recurso que intentó actualizar fue modificado simultáneamente por otro usuario. Por favor recargue la información."),
            _ => (StatusCodes.Status500InternalServerError, 
                "Error interno del servidor", 
                "Ocurrió un error no controlado en el servidor. Comuníquese con el administrador del sistema.")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
