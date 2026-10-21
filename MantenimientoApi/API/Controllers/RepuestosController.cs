using System.Text.Json;
using Abstracciones.DTOs.Repuestos;
using Abstracciones.Interfaces.Flujo;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para la administración del catálogo de repuestos e inventario de bodega.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RepuestosController : ControllerBase
{
    private readonly IServicioRepuesto _servicioRepuesto;

    public RepuestosController(IServicioRepuesto servicioRepuesto)
    {
        _servicioRepuesto = servicioRepuesto;
    }

    /// <summary>
    /// Consulta el inventario completo de repuestos con soporte de filtrado, ordenamiento y encabezado de paginación X-Pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RepuestoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RepuestoDto>>> ObtenerTodos(
        [FromQuery] string? busqueda,
        [FromQuery] string? ordenarPor,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var repuestos = await _servicioRepuesto.ObtenerTodosAsync(cancellationToken);

        // Búsqueda
        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            string b = busqueda.Trim().ToLower();
            repuestos = repuestos.Where(r => r.CodigoRepuesto.ToLower().Contains(b) || r.Nombre.ToLower().Contains(b));
        }

        // Ordenamiento
        repuestos = (ordenarPor?.ToLower()) switch
        {
            "nombre" => repuestos.OrderBy(r => r.Nombre),
            "nombre_desc" => repuestos.OrderByDescending(r => r.Nombre),
            "stock" => repuestos.OrderBy(r => r.Stock),
            "stock_desc" => repuestos.OrderByDescending(r => r.Stock),
            "costo" => repuestos.OrderBy(r => r.CostoUnitario),
            "costo_desc" => repuestos.OrderByDescending(r => r.CostoUnitario),
            "codigo_desc" => repuestos.OrderByDescending(r => r.CodigoRepuesto),
            _ => repuestos.OrderBy(r => r.CodigoRepuesto)
        };

        // Paginación
        int totalItems = repuestos.Count();
        int totalPaginas = (int)Math.Ceiling(totalItems / (double)tamanoPagina);
        pagina = Math.Max(1, pagina);
        var repuestosPaginados = repuestos.Skip((pagina - 1) * tamanoPagina).Take(tamanoPagina).ToList();

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

        return Ok(repuestosPaginados);
    }

    /// <summary>
    /// Consulta los repuestos cuyo nivel de existencias se encuentra en o por debajo del stock mínimo configurado.
    /// </summary>
    [HttpGet("bajo-minimo")]
    [ProducesResponseType(typeof(IEnumerable<RepuestoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RepuestoDto>>> ObtenerBajoStockMinimo(CancellationToken cancellationToken = default)
    {
        var repuestos = await _servicioRepuesto.ObtenerBajoStockMinimoAsync(cancellationToken);
        return Ok(repuestos);
    }

    /// <summary>
    /// Obtiene el detalle de un repuesto específico por su código con soporte de validación de caché ETag.
    /// </summary>
    [HttpGet("{codigoRepuesto:codigoRuta}")]
    [ProducesResponseType(typeof(RepuestoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RepuestoDto>> ObtenerPorCodigo(
        [FromRoute] string codigoRepuesto,
        CancellationToken cancellationToken = default)
    {
        var repuesto = await _servicioRepuesto.ObtenerPorCodigoAsync(codigoRepuesto, cancellationToken);

        string eTag = $"\"{repuesto.RowVersion}\"";

        if (Request.Headers.IfNoneMatch.ToString() == eTag)
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        Response.Headers.ETag = eTag;
        return Ok(repuesto);
    }

    /// <summary>
    /// Registra un nuevo repuesto en el catálogo.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RepuestoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RepuestoDto>> Crear(
        [FromBody] CrearRepuestoDto dto,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _servicioRepuesto.CrearAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorCodigo), new { codigoRepuesto = resultado.CodigoRepuesto }, resultado);
    }

    /// <summary>
    /// Actualiza la información y existencias de un repuesto validando el token de concurrencia optimista.
    /// </summary>
    [HttpPut("{codigoRepuesto:codigoRuta}")]
    [ProducesResponseType(typeof(RepuestoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RepuestoDto>> Actualizar(
        [FromRoute] string codigoRepuesto,
        [FromBody] ActualizarRepuestoDto dto,
        CancellationToken cancellationToken = default)
    {
        // Soporte para encabezado HTTP If-Match en caso de que el cliente prefiera enviarlo en el header
        string? ifMatch = Request.Headers.IfMatch.ToString();
        if (!string.IsNullOrWhiteSpace(ifMatch) && string.IsNullOrWhiteSpace(dto.RowVersion))
        {
            dto.RowVersion = ifMatch.Trim('"');
        }

        var resultado = await _servicioRepuesto.ActualizarAsync(codigoRepuesto, dto, cancellationToken);
        Response.Headers.ETag = $"\"{resultado.RowVersion}\"";
        return Ok(resultado);
    }

    /// <summary>
    /// Elimina un repuesto si no posee registros históricos de consumo en órdenes de trabajo.
    /// </summary>
    [HttpDelete("{codigoRepuesto:codigoRuta}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(
        [FromRoute] string codigoRepuesto,
        CancellationToken cancellationToken = default)
    {
        await _servicioRepuesto.EliminarAsync(codigoRepuesto, cancellationToken);
        return NoContent();
    }
}
