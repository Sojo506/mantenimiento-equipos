using Abstracciones.DTOs.Reportes;
using Abstracciones.Interfaces.Flujo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para la generación de reportes analíticos y exportación en formato CSV (incluyendo SP sp_ReporteConsumoRepuestosPorPeriodo).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Supervisor")]
[Tags("Reportes y Exportación CSV")]
public class ReportesController : ControllerBase
{
    private readonly IServicioReportes _servicioReportes;

    public ReportesController(IServicioReportes servicioReportes)
    {
        _servicioReportes = servicioReportes;
    }

    /// <summary>
    /// Consulta en JSON el reporte de consumo de repuestos por período ejecutando el SP sp_ReporteConsumoRepuestosPorPeriodo.
    /// </summary>
    [HttpGet("consumo-repuestos")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<ReporteConsumoRepuestoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ReporteConsumoRepuestoDto>>> ObtenerConsumoRepuestos(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        CancellationToken cancellationToken = default)
    {
        DateTime inicio = fechaInicio ?? new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime fin = fechaFin ?? DateTime.UtcNow.AddDays(1);
        var resultado = await _servicioReportes.ObtenerConsumoRepuestosAsync(inicio, fin, cancellationToken);
        return Ok(resultado);
    }

    /// <summary>
    /// Exporta en formato CSV el reporte de consumo de repuestos por período (SP sp_ReporteConsumoRepuestosPorPeriodo).
    /// </summary>
    [HttpGet("consumo-repuestos/csv")]
    [Produces("text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DescargarCsvConsumoRepuestos(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        CancellationToken cancellationToken = default)
    {
        DateTime inicio = fechaInicio ?? new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime fin = fechaFin ?? DateTime.UtcNow.AddDays(1);
        byte[] bytes = await _servicioReportes.GenerarCsvConsumoRepuestosAsync(inicio, fin, cancellationToken);
        return File(bytes, "text/csv; charset=utf-8", $"reporte_consumo_repuestos_{inicio:yyyyMMdd}_{fin:yyyyMMdd}.csv");
    }

    /// <summary>
    /// Exporta en formato CSV el reporte consolidado de órdenes de trabajo por estado, prioridad, tiempos y costos.
    /// </summary>
    [HttpGet("ordenes/csv")]
    [Produces("text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DescargarCsvOrdenes(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        CancellationToken cancellationToken = default)
    {
        byte[] bytes = await _servicioReportes.GenerarCsvOrdenesAsync(fechaInicio, fechaFin, cancellationToken);
        return File(bytes, "text/csv; charset=utf-8", $"reporte_ordenes_trabajo_{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    /// <summary>
    /// Exporta en formato CSV el reporte de equipos críticos y reincidencia de fallas (RN-03).
    /// </summary>
    [HttpGet("equipos-criticos/csv")]
    [Produces("text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DescargarCsvEquiposCriticos(CancellationToken cancellationToken = default)
    {
        byte[] bytes = await _servicioReportes.GenerarCsvEquiposCriticosAsync(cancellationToken);
        return File(bytes, "text/csv; charset=utf-8", $"reporte_equipos_criticos_{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    /// <summary>
    /// Exporta en formato CSV el reporte de productividad y carga operativa por técnico (RN-07).
    /// </summary>
    [HttpGet("productividad-tecnicos/csv")]
    [Produces("text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DescargarCsvProductividadTecnicos(CancellationToken cancellationToken = default)
    {
        byte[] bytes = await _servicioReportes.GenerarCsvProductividadTecnicosAsync(cancellationToken);
        return File(bytes, "text/csv; charset=utf-8", $"reporte_productividad_tecnicos_{DateTime.UtcNow:yyyyMMdd}.csv");
    }
}
