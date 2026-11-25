using Abstracciones.DTOs.Planes;
using Abstracciones.Interfaces.Flujo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/procesos")]
[Produces("application/json")]
[Tags("Procesos Automáticos")]
[Authorize(Roles = "Administrador,Supervisor")]
public class ProcesosController : ControllerBase
{
    private readonly IServicioProcesoAutomatico _servicioProceso;

    public ProcesosController(IServicioProcesoAutomatico servicioProceso)
    {
        _servicioProceso = servicioProceso;
    }

    [HttpPost("reglas-automaticas")]
    [EndpointSummary("Ejecutar reglas automáticas RN-01 y RN-06")]
    [EndpointDescription("Ejecuta de inmediato el mismo proceso que corre periódicamente en segundo plano y devuelve las órdenes escaladas y generadas.")]
    [ProducesResponseType(typeof(ResultadoProcesoAutomaticoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResultadoProcesoAutomaticoDto>> EjecutarReglasAutomaticas(CancellationToken cancellationToken = default)
    {
        return Ok(await _servicioProceso.EjecutarAsync(cancellationToken));
    }
}
