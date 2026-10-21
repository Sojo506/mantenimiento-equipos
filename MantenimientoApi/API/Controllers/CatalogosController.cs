using Abstracciones.Entidades;
using Abstracciones.Interfaces.Flujo;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para la consulta de catálogos paramétricos del sistema (estados, tipos, marcas, ubicaciones, etc.).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CatalogosController : ControllerBase
{
    private readonly IServicioCatalogos _servicioCatalogos;

    public CatalogosController(IServicioCatalogos servicioCatalogos)
    {
        _servicioCatalogos = servicioCatalogos;
    }

    /// <summary>
    /// Consulta el catálogo de estados generales (activos, inactivos, estados de órdenes).
    /// </summary>
    [HttpGet("estados")]
    [ProducesResponseType(typeof(IEnumerable<Estado>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Estado>>> ObtenerEstados(CancellationToken cancellationToken = default)
    {
        var lista = await _servicioCatalogos.ObtenerEstadosAsync(cancellationToken);
        return Ok(lista);
    }

    /// <summary>
    /// Consulta el catálogo de tipos de usuario (Administrador, Técnico, Solicitante, etc.).
    /// </summary>
    [HttpGet("tipos-usuario")]
    [ProducesResponseType(typeof(IEnumerable<TipoUsuario>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TipoUsuario>>> ObtenerTiposUsuario(CancellationToken cancellationToken = default)
    {
        var lista = await _servicioCatalogos.ObtenerTiposUsuarioAsync(cancellationToken);
        return Ok(lista);
    }

    /// <summary>
    /// Consulta el catálogo de tipos de equipo.
    /// </summary>
    [HttpGet("tipos-equipo")]
    [ProducesResponseType(typeof(IEnumerable<TipoEquipo>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TipoEquipo>>> ObtenerTiposEquipo(CancellationToken cancellationToken = default)
    {
        var lista = await _servicioCatalogos.ObtenerTiposEquipoAsync(cancellationToken);
        return Ok(lista);
    }

    /// <summary>
    /// Consulta el catálogo de marcas de fabricantes.
    /// </summary>
    [HttpGet("marcas")]
    [ProducesResponseType(typeof(IEnumerable<Marca>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Marca>>> ObtenerMarcas(CancellationToken cancellationToken = default)
    {
        var lista = await _servicioCatalogos.ObtenerMarcasAsync(cancellationToken);
        return Ok(lista);
    }

    /// <summary>
    /// Consulta el catálogo de modelos de equipo.
    /// </summary>
    [HttpGet("modelos")]
    [ProducesResponseType(typeof(IEnumerable<Modelo>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Modelo>>> ObtenerModelos(CancellationToken cancellationToken = default)
    {
        var lista = await _servicioCatalogos.ObtenerModelosAsync(cancellationToken);
        return Ok(lista);
    }

    /// <summary>
    /// Consulta el catálogo de ubicaciones físicas y áreas de trabajo de la empresa.
    /// </summary>
    [HttpGet("ubicaciones")]
    [ProducesResponseType(typeof(IEnumerable<Ubicacion>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Ubicacion>>> ObtenerUbicaciones(CancellationToken cancellationToken = default)
    {
        var lista = await _servicioCatalogos.ObtenerUbicacionesAsync(cancellationToken);
        return Ok(lista);
    }

    /// <summary>
    /// Consulta el catálogo de especialidades técnicas.
    /// </summary>
    [HttpGet("especialidades")]
    [ProducesResponseType(typeof(IEnumerable<Especialidad>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Especialidad>>> ObtenerEspecialidades(CancellationToken cancellationToken = default)
    {
        var lista = await _servicioCatalogos.ObtenerEspecialidadesAsync(cancellationToken);
        return Ok(lista);
    }

    /// <summary>
    /// Consulta el catálogo de tipos de orden de trabajo (Preventiva, Correctiva).
    /// </summary>
    [HttpGet("tipos-orden")]
    [ProducesResponseType(typeof(IEnumerable<TipoOrden>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TipoOrden>>> ObtenerTiposOrden(CancellationToken cancellationToken = default)
    {
        var lista = await _servicioCatalogos.ObtenerTiposOrdenAsync(cancellationToken);
        return Ok(lista);
    }

    /// <summary>
    /// Consulta el catálogo de niveles de gravedad y criticidad de incidentes.
    /// </summary>
    [HttpGet("tipos-gravedad")]
    [ProducesResponseType(typeof(IEnumerable<TipoGravedad>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TipoGravedad>>> ObtenerTiposGravedad(CancellationToken cancellationToken = default)
    {
        var lista = await _servicioCatalogos.ObtenerTiposGravedadAsync(cancellationToken);
        return Ok(lista);
    }
}
