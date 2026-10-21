using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Abstracciones.DTOs.Equipos;
using Abstracciones.Interfaces.Flujo;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para la gestión y administración del parque de equipos de la empresa.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EquiposController : ControllerBase
{
    private readonly IServicioEquipo _servicioEquipo;

    public EquiposController(IServicioEquipo servicioEquipo)
    {
        _servicioEquipo = servicioEquipo;
    }

    /// <summary>
    /// Obtiene el catálogo de equipos con soporte de búsqueda, ordenamiento y encabezados de paginación.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EquipoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EquipoDto>>> ObtenerTodos(
        [FromQuery] string? busqueda,
        [FromQuery] string? ordenarPor,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var equipos = await _servicioEquipo.ObtenerTodosAsync(cancellationToken);

        // Búsqueda
        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            string b = busqueda.Trim().ToLower();
            equipos = equipos.Where(e => e.CodigoEquipo.ToLower().Contains(b) || e.Nombre.ToLower().Contains(b));
        }

        // Ordenamiento
        equipos = (ordenarPor?.ToLower()) switch
        {
            "nombre" => equipos.OrderBy(e => e.Nombre),
            "nombre_desc" => equipos.OrderByDescending(e => e.Nombre),
            "codigo_desc" => equipos.OrderByDescending(e => e.CodigoEquipo),
            _ => equipos.OrderBy(e => e.CodigoEquipo)
        };

        // Paginación
        int totalItems = equipos.Count();
        int totalPaginas = (int)Math.Ceiling(totalItems / (double)tamanoPagina);
        pagina = Math.Max(1, pagina);
        var equiposPaginados = equipos.Skip((pagina - 1) * tamanoPagina).Take(tamanoPagina).ToList();

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

        return Ok(equiposPaginados);
    }

    /// <summary>
    /// Consulta un equipo específico por su código. Soporta validación de caché con ETag y respuesta 304.
    /// </summary>
    [HttpGet("{codigoEquipo:codigoRuta}")]
    [ProducesResponseType(typeof(EquipoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EquipoDto>> ObtenerPorCodigo(
        [FromRoute] string codigoEquipo,
        CancellationToken cancellationToken = default)
    {
        var equipo = await _servicioEquipo.ObtenerPorCodigoAsync(codigoEquipo, cancellationToken);

        // Cálculo de ETag para optimización HTTP
        string rawEtag = $"{equipo.CodigoEquipo}-{equipo.IdEstado}-{equipo.Nombre}";
        string eTag = $"\"{Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawEtag)))[..16]}\"";

        if (Request.Headers.IfNoneMatch.ToString() == eTag)
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        Response.Headers.ETag = eTag;
        return Ok(equipo);
    }

    /// <summary>
    /// Registra un nuevo equipo en el parque empresarial.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EquipoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EquipoDto>> Crear(
        [FromBody] CrearEquipoDto dto,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _servicioEquipo.CrearAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorCodigo), new { codigoEquipo = resultado.CodigoEquipo }, resultado);
    }

    /// <summary>
    /// Actualiza los datos de un equipo existente.
    /// </summary>
    [HttpPut("{codigoEquipo:codigoRuta}")]
    [ProducesResponseType(typeof(EquipoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EquipoDto>> Actualizar(
        [FromRoute] string codigoEquipo,
        [FromBody] ActualizarEquipoDto dto,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _servicioEquipo.ActualizarAsync(codigoEquipo, dto, cancellationToken);
        return Ok(resultado);
    }

    /// <summary>
    /// Elimina un equipo si no posee órdenes de trabajo asociadas.
    /// </summary>
    [HttpDelete("{codigoEquipo:codigoRuta}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(
        [FromRoute] string codigoEquipo,
        CancellationToken cancellationToken = default)
    {
        await _servicioEquipo.EliminarAsync(codigoEquipo, cancellationToken);
        return NoContent();
    }
}
