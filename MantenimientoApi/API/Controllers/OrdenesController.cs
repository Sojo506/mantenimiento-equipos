using System.Text.Json;
using Abstracciones.DTOs.Ordenes;
using Abstracciones.Interfaces.Flujo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para la gestión y ciclo de vida de órdenes de trabajo de mantenimiento.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Órdenes de Trabajo")]
[Authorize]
public class OrdenesController : ControllerBase
{
    private static readonly string[] PrioridadesValidas = { "Baja", "Media", "Alta", "Critica" };
    private static readonly string[] OrdenamientosValidos = { "fecha", "fecha_desc", "prioridad", "limite", "numero_asc", "numero_desc" };

    private readonly IServicioOrdenDeTrabajo _servicioOrden;

    public OrdenesController(IServicioOrdenDeTrabajo servicioOrden)
    {
        _servicioOrden = servicioOrden;
    }

    /// <summary>
    /// Consulta el listado de órdenes de trabajo con filtros por equipo, técnico, solicitante, estado, prioridad y paginación.
    /// </summary>
    [HttpGet]
    [EndpointSummary("Listar órdenes de trabajo")]
    [EndpointDescription("Filtros: codigoEquipo, identificacionTecnico, identificacionSolicitante, idEstado (7-12), prioridad (Baja, Media, Alta, Critica), busqueda, activas (excluye Cerradas) y vencidas (fecha límite superada y no Cerrada). ordenarPor: fecha, fecha_desc, prioridad, limite, numero_asc, numero_desc. tamanoPagina entre 1 y 50. Los metadatos de paginación viajan en el encabezado X-Pagination.")]
    [ProducesResponseType(typeof(IEnumerable<OrdenDeTrabajoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<OrdenDeTrabajoDto>>> ObtenerTodas(
        [FromQuery] string? codigoEquipo,
        [FromQuery] string? identificacionTecnico,
        [FromQuery] string? identificacionSolicitante,
        [FromQuery] int? idEstado,
        [FromQuery] string? prioridad,
        [FromQuery] string? busqueda,
        [FromQuery] string? ordenarPor,
        [FromQuery] bool activas = false,
        [FromQuery] bool vencidas = false,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        if (pagina < 1)
            ModelState.AddModelError(nameof(pagina), "La página debe ser mayor o igual a 1.");
        if (tamanoPagina is < 1 or > 50)
            ModelState.AddModelError(nameof(tamanoPagina), "El tamaño de página debe estar entre 1 y 50.");
        if (idEstado.HasValue && idEstado is < 7 or > 12)
            ModelState.AddModelError(nameof(idEstado), "El estado de orden debe estar entre 7 (Abierta) y 12 (Escalada).");
        if (!string.IsNullOrWhiteSpace(prioridad) && !PrioridadesValidas.Contains(prioridad, StringComparer.OrdinalIgnoreCase))
            ModelState.AddModelError(nameof(prioridad), "La prioridad debe ser: Baja, Media, Alta o Critica.");
        if (!string.IsNullOrWhiteSpace(ordenarPor) && !OrdenamientosValidos.Contains(ordenarPor.ToLowerInvariant()))
            ModelState.AddModelError(nameof(ordenarPor), $"Ordenamiento no válido. Valores permitidos: {string.Join(", ", OrdenamientosValidos)}.");
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

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

        if (activas)
        {
            ordenes = ordenes.Where(o => o.IdEstado != 11);
        }

        if (vencidas)
        {
            DateTime ahora = DateTime.UtcNow;
            ordenes = ordenes.Where(o => o.IdEstado != 11 && o.FechaLimite < ahora);
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
    [HttpGet("{numero:int:min(1)}")]
    [EndpointSummary("Consultar orden de trabajo")]
    [EndpointDescription("Devuelve el ETag con el RowVersion de la orden. Si se envía If-None-Match con el mismo valor responde 304.")]
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
    [EndpointSummary("Crear orden de trabajo")]
    [EndpointDescription("Calcula la fecha límite según la prioridad (RN-08). 400 si el equipo está 'Dado de Baja', ya tiene una correctiva abierta (RN-04) o el técnico alcanzó su límite (RN-07). 404 si el equipo, solicitante o técnico no existen.")]
    [ProducesResponseType(typeof(OrdenDeTrabajoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrdenDeTrabajoDto>> Crear(
        [FromBody] CrearOrdenDto dto,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _servicioOrden.CrearAsync(dto, cancellationToken);
        Response.Headers.ETag = $"\"{resultado.RowVersion}\"";
        return CreatedAtAction(nameof(ObtenerPorNumero), new { numero = resultado.Numero }, resultado);
    }

    /// <summary>
    /// Actualiza la información operativa de una orden de trabajo (prioridad, descripción, técnico, tiempo invertido).
    /// </summary>
    [HttpPut("{numero:int:min(1)}")]
    [Authorize(Roles = "Administrador,Supervisor,Tecnico")]
    [EndpointSummary("Actualizar orden de trabajo")]
    [EndpointDescription("Requiere el RowVersion en el cuerpo o en If-Match. 409 si la orden cambió desde que se leyó o si está Cerrada (RN-09).")]
    [ProducesResponseType(typeof(OrdenDeTrabajoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrdenDeTrabajoDto>> Actualizar(
        [FromRoute] int numero,
        [FromBody] ActualizarOrdenDto dto,
        CancellationToken cancellationToken = default)
    {
        dto.RowVersion = ResolverRowVersion(dto.RowVersion) ?? string.Empty;

        var resultado = await _servicioOrden.ActualizarAsync(numero, dto, cancellationToken);
        Response.Headers.ETag = $"\"{resultado.RowVersion}\"";
        return Ok(resultado);
    }

    /// <summary>
    /// Transiciona el estado de la orden de trabajo mediante la máquina de estados estricta y registra en bitácora inmutable.
    /// </summary>
    [HttpPut("{numero:int:min(1)}/estado")]
    [Authorize(Roles = "Administrador,Supervisor,Tecnico")]
    [EndpointSummary("Cambiar estado de la orden")]
    [EndpointDescription("Aplica la máquina de estados (7 Abierta, 8 Asignada, 9 En Proceso, 10 En Espera de Repuesto, 11 Cerrada, 12 Escalada). 400 si no se cumple RN-02 al cerrar o RN-07 al iniciar. 409 si la transición no es válida, la orden está Cerrada (RN-09) o el RowVersion no coincide.")]
    [ProducesResponseType(typeof(OrdenDeTrabajoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrdenDeTrabajoDto>> CambiarEstado(
        [FromRoute] int numero,
        [FromBody] CambiarEstadoOrdenDto dto,
        CancellationToken cancellationToken = default)
    {
        dto.RowVersion = ResolverRowVersion(dto.RowVersion) ?? string.Empty;

        if (dto.NuevoEstado == 11 && dto.TiempoInvertidoFinal is < 0)
        {
            ModelState.AddModelError(nameof(dto.TiempoInvertidoFinal), "El tiempo invertido no puede ser negativo.");
            return ValidationProblem(ModelState);
        }

        var resultado = await _servicioOrden.CambiarEstadoAsync(numero, dto, cancellationToken);
        Response.Headers.ETag = $"\"{resultado.RowVersion}\"";
        return Ok(resultado);
    }

    [HttpPut("{numero:int:min(1)}/asignacion")]
    [Authorize(Roles = "Administrador,Supervisor")]
    [EndpointSummary("Asignar técnico a la orden")]
    [EndpointDescription("Una orden Abierta o Escalada pasa a Asignada; en otros estados solo cambia el responsable. 400 si el usuario no es un técnico activo o supera su límite (RN-07). 409 si la orden está Cerrada o el RowVersion no coincide.")]
    [ProducesResponseType(typeof(OrdenDeTrabajoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrdenDeTrabajoDto>> AsignarTecnico(
        [FromRoute] int numero,
        [FromBody] AsignarTecnicoOrdenDto dto,
        CancellationToken cancellationToken = default)
    {
        dto.RowVersion = ResolverRowVersion(dto.RowVersion) ?? string.Empty;

        var resultado = await _servicioOrden.AsignarTecnicoAsync(numero, dto, cancellationToken);
        Response.Headers.ETag = $"\"{resultado.RowVersion}\"";
        return Ok(resultado);
    }

    /// <summary>
    /// Asigna repuestos a la orden de trabajo descontando existencias de bodega y recalculando costos.
    /// </summary>
    [HttpPost("{numero:int:min(1)}/repuestos")]
    [Authorize(Roles = "Administrador,Supervisor,Tecnico")]
    [EndpointSummary("Asignar repuesto a la orden")]
    [EndpointDescription("Descuenta el stock (RN-05). 400 si el stock es insuficiente. 409 si la orden está Cerrada, si el RowVersion enviado (cuerpo o If-Match) no coincide, o si el repuesto fue modificado al mismo tiempo por otro usuario.")]
    [ProducesResponseType(typeof(OrdenDeTrabajoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrdenDeTrabajoDto>> AsignarRepuesto(
        [FromRoute] int numero,
        [FromBody] AsignarRepuestoOrdenDto dto,
        CancellationToken cancellationToken = default)
    {
        dto.RowVersion = ResolverRowVersion(dto.RowVersion);

        var resultado = await _servicioOrden.AsignarRepuestoAsync(numero, dto, cancellationToken);
        Response.Headers.ETag = $"\"{resultado.RowVersion}\"";
        return Ok(resultado);
    }

    /// <summary>
    /// Consulta el historial inmutable de auditoría y bitácora de eventos de la orden de trabajo.
    /// </summary>
    [HttpGet("{numero:int:min(1)}/historial")]
    [EndpointSummary("Historial de bitácora de la orden")]
    [EndpointDescription("Eventos en orden cronológico ascendente.")]
    [ProducesResponseType(typeof(IEnumerable<BitacoraOrdenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<BitacoraOrdenDto>>> ObtenerHistorial(
        [FromRoute] int numero,
        CancellationToken cancellationToken = default)
    {
        var historial = await _servicioOrden.ObtenerHistorialBitacoraAsync(numero, cancellationToken);

        var dtos = historial
            .OrderBy(b => b.Fecha)
            .ThenBy(b => b.IdBitacora)
            .Select(b => new BitacoraOrdenDto
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

    private string? ResolverRowVersion(string? rowVersionCuerpo)
    {
        if (!string.IsNullOrWhiteSpace(rowVersionCuerpo))
            return rowVersionCuerpo;

        string ifMatch = Request.Headers.IfMatch.ToString().Trim();
        if (string.IsNullOrWhiteSpace(ifMatch) || ifMatch == "*")
            return rowVersionCuerpo;

        if (ifMatch.StartsWith("W/", StringComparison.Ordinal))
            ifMatch = ifMatch[2..];

        return ifMatch.Trim('"');
    }
}
