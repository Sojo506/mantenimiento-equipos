using Abstracciones.DTOs.Catalogos;
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
    [ProducesResponseType(typeof(IEnumerable<ElementoCatalogoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ElementoCatalogoDto>>> ObtenerEstados(CancellationToken cancellationToken = default)
    {
        var lista = await _servicioCatalogos.ObtenerEstadosAsync(cancellationToken);
        return Ok(lista.Select(e => new ElementoCatalogoDto { Id = e.IdEstado, Nombre = e.Nombre }));
    }

    /// <summary>
    /// Consulta el catálogo de tipos de usuario (Administrador, Técnico, Solicitante, etc.).
    /// </summary>
    [HttpGet("tipos-usuario")]
    [ProducesResponseType(typeof(IEnumerable<ElementoCatalogoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ElementoCatalogoDto>>> ObtenerTiposUsuario(CancellationToken cancellationToken = default)
    {
        var lista = await _servicioCatalogos.ObtenerTiposUsuarioAsync(cancellationToken);
        return Ok(lista.Select(t => new ElementoCatalogoDto { Id = t.IdTipoUsuario, Nombre = t.Nombre }));
    }

    /// <summary>
    /// Consulta el catálogo de tipos de equipo.
    /// </summary>
    [HttpGet("tipos-equipo")]
    [ProducesResponseType(typeof(IEnumerable<ElementoCatalogoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ElementoCatalogoDto>>> ObtenerTiposEquipo(CancellationToken cancellationToken = default)
    {
        var lista = await _servicioCatalogos.ObtenerTiposEquipoAsync(cancellationToken);
        return Ok(lista.Select(t => new ElementoCatalogoDto { Id = t.IdTipoEquipo, Nombre = t.Nombre }));
    }

    /// <summary>
    /// Consulta el catálogo de marcas de fabricantes.
    /// </summary>
    [HttpGet("marcas")]
    [ProducesResponseType(typeof(IEnumerable<ElementoCatalogoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ElementoCatalogoDto>>> ObtenerMarcas(CancellationToken cancellationToken = default)
    {
        var lista = await _servicioCatalogos.ObtenerMarcasAsync(cancellationToken);
        return Ok(lista.Select(m => new ElementoCatalogoDto { Id = m.IdMarca, Nombre = m.Nombre }));
    }

    /// <summary>
    /// Consulta el catálogo de modelos de equipo.
    /// </summary>
    [HttpGet("modelos")]
    [ProducesResponseType(typeof(IEnumerable<ElementoCatalogoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ElementoCatalogoDto>>> ObtenerModelos(CancellationToken cancellationToken = default)
    {
        var lista = await _servicioCatalogos.ObtenerModelosAsync(cancellationToken);
        return Ok(lista.Select(m => new ElementoCatalogoDto { Id = m.IdModelo, Nombre = m.Nombre }));
    }

    /// <summary>
    /// Consulta el catálogo de ubicaciones físicas y áreas de trabajo de la empresa.
    /// </summary>
    [HttpGet("ubicaciones")]
    [ProducesResponseType(typeof(IEnumerable<ElementoCatalogoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ElementoCatalogoDto>>> ObtenerUbicaciones(CancellationToken cancellationToken = default)
    {
        var lista = await _servicioCatalogos.ObtenerUbicacionesAsync(cancellationToken);
        return Ok(lista.Select(u => new ElementoCatalogoDto { Id = u.IdUbicacion, Nombre = u.Nombre }));
    }

    /// <summary>
    /// Consulta el catálogo de especialidades técnicas.
    /// </summary>
    [HttpGet("especialidades")]
    [ProducesResponseType(typeof(IEnumerable<ElementoCatalogoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ElementoCatalogoDto>>> ObtenerEspecialidades(CancellationToken cancellationToken = default)
    {
        var lista = await _servicioCatalogos.ObtenerEspecialidadesAsync(cancellationToken);
        return Ok(lista.Select(e => new ElementoCatalogoDto { Id = e.IdEspecialidad, Nombre = e.Nombre }));
    }

    /// <summary>
    /// Consulta el catálogo de tipos de orden de trabajo (Preventiva, Correctiva).
    /// </summary>
    [HttpGet("tipos-orden")]
    [ProducesResponseType(typeof(IEnumerable<ElementoCatalogoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ElementoCatalogoDto>>> ObtenerTiposOrden(CancellationToken cancellationToken = default)
    {
        var lista = await _servicioCatalogos.ObtenerTiposOrdenAsync(cancellationToken);
        return Ok(lista.Select(t => new ElementoCatalogoDto { Id = t.IdTipoOrden, Nombre = t.Nombre }));
    }

    /// <summary>
    /// Consulta el catálogo de niveles de gravedad y criticidad de incidentes.
    /// </summary>
    [HttpGet("tipos-gravedad")]
    [ProducesResponseType(typeof(IEnumerable<ElementoCatalogoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ElementoCatalogoDto>>> ObtenerTiposGravedad(CancellationToken cancellationToken = default)
    {
        var lista = await _servicioCatalogos.ObtenerTiposGravedadAsync(cancellationToken);
        return Ok(lista.Select(g => new ElementoCatalogoDto { Id = g.IdTipoGravedad, Nombre = g.Nombre }));
    }
}
