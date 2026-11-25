using Abstracciones.DTOs.Fallas;
using Abstracciones.Interfaces.Flujo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para el reporte y consulta de fallas registradas sobre equipos (incluye regla RN-03).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class FallasController : ControllerBase
{
    private readonly IServicioFalla _servicioFalla;

    public FallasController(IServicioFalla servicioFalla)
    {
        _servicioFalla = servicioFalla;
    }

    /// <summary>
    /// Obtiene el listado histórico de fallas registradas en el sistema.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FallaRegistradaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FallaRegistradaDto>>> ObtenerTodas(
        [FromQuery] string? codigoEquipo,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(codigoEquipo))
        {
            var porEquipo = await _servicioFalla.ObtenerPorEquipoAsync(codigoEquipo, cancellationToken);
            return Ok(porEquipo);
        }

        var fallas = await _servicioFalla.ObtenerTodasAsync(cancellationToken);
        return Ok(fallas);
    }

    /// <summary>
    /// Obtiene las fallas registradas de un equipo específico por su código.
    /// </summary>
    [HttpGet("equipo/{codigoEquipo:codigoRuta}")]
    [ProducesResponseType(typeof(IEnumerable<FallaRegistradaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<FallaRegistradaDto>>> ObtenerPorEquipo(
        [FromRoute] string codigoEquipo,
        CancellationToken cancellationToken = default)
    {
        var fallas = await _servicioFalla.ObtenerPorEquipoAsync(codigoEquipo, cancellationToken);
        return Ok(fallas);
    }

    /// <summary>
    /// Consulta el detalle de una falla registrada por su ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FallaRegistradaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FallaRegistradaDto>> ObtenerPorId(
        [FromRoute] int id,
        CancellationToken cancellationToken = default)
    {
        var falla = await _servicioFalla.ObtenerPorIdAsync(id, cancellationToken);
        return Ok(falla);
    }

    /// <summary>
    /// Registra una nueva falla para un equipo y evalúa la regla RN-03 (paso automático a estado EnRiesgo).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(FallaRegistradaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FallaRegistradaDto>> Registrar(
        [FromBody] RegistrarFallaDto dto,
        CancellationToken cancellationToken = default)
    {
        var creada = await _servicioFalla.RegistrarAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.Id }, creada);
    }
}
