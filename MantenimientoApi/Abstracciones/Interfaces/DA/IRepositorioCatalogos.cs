using Abstracciones.Entidades;

namespace Abstracciones.Interfaces.DA;

/// <summary>
/// Contrato de acceso a datos para la consulta de catálogos generales del sistema.
/// </summary>
public interface IRepositorioCatalogos
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
