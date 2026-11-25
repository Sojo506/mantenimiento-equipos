using System.Text.Json;
using Abstracciones.DTOs.Planes;
using Abstracciones.Interfaces.Flujo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/planes")]
[Produces("application/json")]
[Tags("Planes de Mantenimiento")]
[Authorize]
public class PlanesController : ControllerBase
{
    private const string RolesGestion = "Administrador,Supervisor";
    private static readonly string[] OrdenamientosValidos = { "proxima", "proxima_desc", "equipo", "frecuencia", "id" };

    private readonly IServicioPlanMantenimiento _servicioPlan;

    public PlanesController(IServicioPlanMantenimiento servicioPlan)
    {
        _servicioPlan = servicioPlan;
    }

    [HttpGet]
    [EndpointSummary("Listar planes de mantenimiento")]
    [EndpointDescription("Filtros: codigoEquipo, idEstado (1 Activo, 2 Inactivo), soloVencidos y busqueda. ordenarPor: proxima, proxima_desc, equipo, frecuencia, id. tamanoPagina entre 1 y 50.")]
    [ProducesResponseType(typeof(IEnumerable<PlanMantenimientoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<PlanMantenimientoDto>>> ObtenerTodos(
        [FromQuery] string? codigoEquipo,
        [FromQuery] int? idEstado,
        [FromQuery] bool soloVencidos = false,
        [FromQuery] string? busqueda = null,
        [FromQuery] string? ordenarPor = null,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        if (pagina < 1)
            ModelState.AddModelError(nameof(pagina), "La página debe ser mayor o igual a 1.");
        if (tamanoPagina is < 1 or > 50)
            ModelState.AddModelError(nameof(tamanoPagina), "El tamaño de página debe estar entre 1 y 50.");
        if (idEstado.HasValue && idEstado is not (1 or 2))
            ModelState.AddModelError(nameof(idEstado), "El estado del plan debe ser 1 (Activo) o 2 (Inactivo).");
        if (!string.IsNullOrWhiteSpace(ordenarPor) && !OrdenamientosValidos.Contains(ordenarPor.ToLowerInvariant()))
            ModelState.AddModelError(nameof(ordenarPor), $"Ordenamiento no válido. Valores permitidos: {string.Join(", ", OrdenamientosValidos)}.");
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        IEnumerable<PlanMantenimientoDto> planes = await _servicioPlan.ObtenerTodosAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(codigoEquipo))
            planes = planes.Where(p => p.CodigoEquipo.Equals(codigoEquipo.Trim(), StringComparison.OrdinalIgnoreCase));
        if (idEstado.HasValue)
            planes = planes.Where(p => p.IdEstado == idEstado.Value);
        if (soloVencidos)
            planes = planes.Where(p => p.Vencido);
        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            string b = busqueda.Trim();
            planes = planes.Where(p =>
                p.CodigoEquipo.Contains(b, StringComparison.OrdinalIgnoreCase) ||
                p.EquipoNombre.Contains(b, StringComparison.OrdinalIgnoreCase));
        }

        planes = ordenarPor?.ToLowerInvariant() switch
        {
            "proxima_desc" => planes.OrderByDescending(p => p.ProximaFecha),
            "equipo" => planes.OrderBy(p => p.CodigoEquipo),
            "frecuencia" => planes.OrderBy(p => p.Frecuencia),
            "id" => planes.OrderBy(p => p.IdPlanMantenimiento),
            _ => planes.OrderBy(p => p.ProximaFecha)
        };

        var lista = planes.ToList();
        int totalPaginas = (int)Math.Ceiling(lista.Count / (double)tamanoPagina);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new
        {
            TotalItems = lista.Count,
            PageNumber = pagina,
            PageSize = tamanoPagina,
            TotalPages = totalPaginas,
            HasPrevious = pagina > 1,
            HasNext = pagina < totalPaginas
        }));

        return Ok(lista.Skip((pagina - 1) * tamanoPagina).Take(tamanoPagina));
    }

    [HttpGet("{idPlan:int:min(1)}")]
    [EndpointSummary("Consultar plan de mantenimiento")]
    [ProducesResponseType(typeof(PlanMantenimientoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlanMantenimientoDto>> ObtenerPorId(
        [FromRoute] int idPlan,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _servicioPlan.ObtenerPorIdAsync(idPlan, cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = RolesGestion)]
    [EndpointSummary("Crear plan de mantenimiento")]
    [EndpointDescription("Frecuencia en días (1-365). El equipo debe existir, no estar 'Dado de Baja' (400) y no tener otro plan (409).")]
    [ProducesResponseType(typeof(PlanMantenimientoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PlanMantenimientoDto>> Crear(
        [FromBody] CrearPlanMantenimientoDto dto,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _servicioPlan.CrearAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorId), new { idPlan = resultado.IdPlanMantenimiento }, resultado);
    }

    [HttpPut("{idPlan:int:min(1)}")]
    [Authorize(Roles = RolesGestion)]
    [EndpointSummary("Actualizar plan de mantenimiento")]
    [ProducesResponseType(typeof(PlanMantenimientoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlanMantenimientoDto>> Actualizar(
        [FromRoute] int idPlan,
        [FromBody] ActualizarPlanMantenimientoDto dto,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _servicioPlan.ActualizarAsync(idPlan, dto, cancellationToken));
    }

    [HttpDelete("{idPlan:int:min(1)}")]
    [Authorize(Roles = RolesGestion)]
    [EndpointSummary("Eliminar plan de mantenimiento")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(
        [FromRoute] int idPlan,
        CancellationToken cancellationToken = default)
    {
        await _servicioPlan.EliminarAsync(idPlan, cancellationToken);
        return NoContent();
    }
}
