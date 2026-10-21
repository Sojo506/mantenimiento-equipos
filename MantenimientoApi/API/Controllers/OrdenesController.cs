using System.Text.Json;
using Abstracciones.DTOs.Ordenes;
using Abstracciones.Interfaces.Flujo;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para la gestión y ciclo de vida de órdenes de trabajo de mantenimiento.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdenesController : ControllerBase
{
    private readonly IServicioOrdenDeTrabajo _servicioOrden;

    public OrdenesController(IServicioOrdenDeTrabajo servicioOrden)
    {
        _servicioOrden = servicioOrden;
    }

    /// <summary>
    /// Consulta el listado de órdenes de trabajo con filtros por equipo, técnico, solicitante, estado, prioridad y paginación.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrdenDeTrabajoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrdenDeTrabajoDto>>> ObtenerTodas(
        [FromQuery] string? codigoEquipo,
        [FromQuery] string? identificacionTecnico,
        [FromQuery] string? identificacionSolicitante,
        [FromQuery] int? idEstado,
        [FromQuery] string? prioridad,
        [FromQuery] string? busqueda,
        [FromQuery] string? ordenarPor,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<OrdenDeTrabajoDto> ordenes;

        // Selección de origen según filtros prioritarios
        if (!string.IsNullOrWhiteSpace(codigoEquipo))
        {
            ordenes = await _servicioOrden.ObtenerPorEquipoAsync(codigoEquipo, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(identificacionTecnico))
        {
            ordenes = await _servicioOrden.ObtenerPorTecnicoAsync(identificacionTecnico, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(identificacionSolicitante))
        {
            ordenes = await _servicioOrden.ObtenerPorSolicitanteAsync(identificacionSolicitante, cancellationToken);
        }
        else
        {
            ordenes = await _servicioOrden.ObtenerTodasAsync(cancellationToken);
        }

        // Filtros adicionales
        if (idEstado.HasValue)
        {
            ordenes = ordenes.Where(o => o.IdEstado == idEstado.Value);
        }

        if (!string.IsNullOrWhiteSpace(prioridad))
        {
            ordenes = ordenes.Where(o => o.Prioridad.Equals(prioridad, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            string b = busqueda.Trim().ToLower();
            ordenes = ordenes.Where(o =>
                o.Numero.ToString().Contains(b) ||
                o.CodigoEquipo.ToLower().Contains(b) ||
                o.Descripcion.ToLower().Contains(b) ||
                (o.TecnicoNombre != null && o.TecnicoNombre.ToLower().Contains(b)));
        }

        // Ordenamiento
        ordenes = (ordenarPor?.ToLower()) switch
        {
            "fecha" => ordenes.OrderBy(o => o.FechaCreacion),
            "fecha_desc" => ordenes.OrderByDescending(o => o.FechaCreacion),
            "prioridad" => ordenes.OrderBy(o => o.Prioridad),
            "limite" => ordenes.OrderBy(o => o.FechaLimite),
            "numero_asc" => ordenes.OrderBy(o => o.Numero),
            _ => ordenes.OrderByDescending(o => o.Numero)
        };

        // Paginación
        int totalItems = ordenes.Count();
        int totalPaginas = (int)Math.Ceiling(totalItems / (double)tamanoPagina);
        pagina = Math.Max(1, pagina);
        var ordenesPaginadas = ordenes.Skip((pagina - 1) * tamanoPagina).Take(tamanoPagina).ToList();

        var metadataPaginacion = new
        {
            TotalItems = totalItems,
            PageNumber = pagina,
            PageSize = tamanoPagina,
            TotalPages = totalPaginas,
            HasPrevious = pagina > 1,
            HasNext = pagina < totalPaginas
        };

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadataPaginacion));

        return Ok(ordenesPaginadas);
    }

    /// <summary>
    /// Consulta una orden de trabajo por su número correlativo. Soporta validación de caché ETag.
    /// </summary>
    [HttpGet("{numero:int}")]
    [ProducesResponseType(typeof(OrdenDeTrabajoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrdenDeTrabajoDto>> ObtenerPorNumero(
        [FromRoute] int numero,
        CancellationToken cancellationToken = default)
    {
        var orden = await _servicioOrden.ObtenerPorNumeroAsync(numero, cancellationToken);

        string eTag = $"\"{orden.RowVersion}\"";

        if (Request.Headers.IfNoneMatch.ToString() == eTag)
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        Response.Headers.ETag = eTag;
        return Ok(orden);
    }

    /// <summary>
    /// Crea y registra una nueva orden de trabajo, validando reglas de negocio e insertando la bitácora inicial.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrdenDeTrabajoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrdenDeTrabajoDto>> Crear(
        [FromBody] CrearOrdenDto dto,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _servicioOrden.CrearAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorNumero), new { numero = resultado.Numero }, resultado);
    }

    /// <summary>
    /// Actualiza la información operativa de una orden de trabajo (prioridad, descripción, técnico, tiempo invertido).
    /// </summary>
    [HttpPut("{numero:int}")]
    [ProducesResponseType(typeof(OrdenDeTrabajoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrdenDeTrabajoDto>> Actualizar(
        [FromRoute] int numero,
        [FromBody] ActualizarOrdenDto dto,
        CancellationToken cancellationToken = default)
    {
        string? ifMatch = Request.Headers.IfMatch.ToString();
        if (!string.IsNullOrWhiteSpace(ifMatch) && string.IsNullOrWhiteSpace(dto.RowVersion))
        {
            dto.RowVersion = ifMatch.Trim('"');
        }

        var resultado = await _servicioOrden.ActualizarAsync(numero, dto, cancellationToken);
        Response.Headers.ETag = $"\"{resultado.RowVersion}\"";
        return Ok(resultado);
    }

    /// <summary>
    /// Transiciona el estado de la orden de trabajo mediante la máquina de estados estricta y registra en bitácora inmutable.
    /// </summary>
    [HttpPut("{numero:int}/estado")]
    [ProducesResponseType(typeof(OrdenDeTrabajoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrdenDeTrabajoDto>> CambiarEstado(
        [FromRoute] int numero,
        [FromBody] CambiarEstadoOrdenDto dto,
        CancellationToken cancellationToken = default)
    {
        string? ifMatch = Request.Headers.IfMatch.ToString();
        if (!string.IsNullOrWhiteSpace(ifMatch) && string.IsNullOrWhiteSpace(dto.RowVersion))
        {
            dto.RowVersion = ifMatch.Trim('"');
        }

        var resultado = await _servicioOrden.CambiarEstadoAsync(numero, dto, cancellationToken);
        Response.Headers.ETag = $"\"{resultado.RowVersion}\"";
        return Ok(resultado);
    }

    /// <summary>
    /// Asigna repuestos a la orden de trabajo descontando existencias de bodega y recalculando costos.
    /// </summary>
    [HttpPost("{numero:int}/repuestos")]
    [ProducesResponseType(typeof(OrdenDeTrabajoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrdenDeTrabajoDto>> AsignarRepuesto(
        [FromRoute] int numero,
        [FromBody] AsignarRepuestoOrdenDto dto,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _servicioOrden.AsignarRepuestoAsync(numero, dto, cancellationToken);
        Response.Headers.ETag = $"\"{resultado.RowVersion}\"";
        return Ok(resultado);
    }

    /// <summary>
    /// Consulta el historial inmutable de auditoría y bitácora de eventos de la orden de trabajo.
    /// </summary>
    [HttpGet("{numero:int}/historial")]
    [ProducesResponseType(typeof(IEnumerable<BitacoraOrdenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<BitacoraOrdenDto>>> ObtenerHistorial(
        [FromRoute] int numero,
        CancellationToken cancellationToken = default)
    {
        var historial = await _servicioOrden.ObtenerHistorialBitacoraAsync(numero, cancellationToken);

        var dtos = historial.Select(b => new BitacoraOrdenDto
        {
            IdBitacora = b.IdBitacora,
            Numero = b.Numero,
            Identificacion = b.Identificacion,
            NombreUsuario = b.Usuario?.Nombre ?? b.Identificacion,
            Fecha = b.Fecha,
            EstadoAnterior = b.EstadoAnterior,
            EstadoNuevo = b.EstadoNuevo,
            Comentario = b.Comentario
        });

        return Ok(dtos);
    }
}
