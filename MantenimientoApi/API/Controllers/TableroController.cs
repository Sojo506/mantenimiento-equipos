using Abstracciones.DTOs.Tablero;
using Abstracciones.Interfaces.Flujo;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/tablero")]
[Produces("application/json")]
[Tags("Tablero de Control")]
public class TableroController : ControllerBase
{
    private readonly IServicioTablero _servicioTablero;

    public TableroController(IServicioTablero servicioTablero)
    {
        _servicioTablero = servicioTablero;
    }

    [HttpGet("equipos-por-estado")]
    [ProducesResponseType(
        typeof(IEnumerable<EquipoPorEstadoDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerEquiposPorEstado(
        CancellationToken cancellationToken)
    {
        var resultado =
            await _servicioTablero.ObtenerEquiposPorEstadoAsync(
                cancellationToken);

        return Ok(resultado);
    }

    [HttpGet("ordenes-por-prioridad")]
    [ProducesResponseType(
        typeof(IEnumerable<OrdenPorPrioridadDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerOrdenesPorPrioridad(
        CancellationToken cancellationToken)
    {
        var resultado =
            await _servicioTablero.ObtenerOrdenesPorPrioridadAsync(
                cancellationToken);

        return Ok(resultado);
    }

    [HttpGet("repuestos-bajo-minimo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerRepuestosBajoMinimo(
        CancellationToken cancellationToken)
    {
        var items =
            await _servicioTablero.ObtenerRepuestosBajoMinimoAsync(
                cancellationToken);

        return Ok(new
        {
            TotalCriticos = items.Count(),
            Items = items
        });
    }
}