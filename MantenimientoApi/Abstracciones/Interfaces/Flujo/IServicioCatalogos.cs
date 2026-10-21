using Abstracciones.Entidades;

namespace Abstracciones.Interfaces.Flujo;

/// <summary>
/// Contrato para la consulta de catálogos generales en la capa de negocio.
/// </summary>
public interface IServicioCatalogos
{
    Task<IEnumerable<Estado>> ObtenerEstadosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TipoUsuario>> ObtenerTiposUsuarioAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TipoEquipo>> ObtenerTiposEquipoAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Marca>> ObtenerMarcasAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Modelo>> ObtenerModelosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Ubicacion>> ObtenerUbicacionesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Especialidad>> ObtenerEspecialidadesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TipoOrden>> ObtenerTiposOrdenAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TipoGravedad>> ObtenerTiposGravedadAsync(CancellationToken cancellationToken = default);
}
