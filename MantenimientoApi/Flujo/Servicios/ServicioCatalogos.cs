using Abstracciones.Entidades;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;

namespace Flujo.Servicios;

/// <summary>
/// Implementación del servicio de consulta de catálogos generales en la capa de negocio.
/// </summary>
public class ServicioCatalogos : IServicioCatalogos
{
    private readonly IRepositorioCatalogos _repositorioCatalogos;

    public ServicioCatalogos(IRepositorioCatalogos repositorioCatalogos)
    {
        _repositorioCatalogos = repositorioCatalogos;
    }

    public Task<IEnumerable<Estado>> ObtenerEstadosAsync(CancellationToken cancellationToken = default)
    {
        return _repositorioCatalogos.ObtenerEstadosAsync(cancellationToken);
    }

    public Task<IEnumerable<TipoUsuario>> ObtenerTiposUsuarioAsync(CancellationToken cancellationToken = default)
    {
        return _repositorioCatalogos.ObtenerTiposUsuarioAsync(cancellationToken);
    }

    public Task<IEnumerable<TipoEquipo>> ObtenerTiposEquipoAsync(CancellationToken cancellationToken = default)
    {
        return _repositorioCatalogos.ObtenerTiposEquipoAsync(cancellationToken);
    }

    public Task<IEnumerable<Marca>> ObtenerMarcasAsync(CancellationToken cancellationToken = default)
    {
        return _repositorioCatalogos.ObtenerMarcasAsync(cancellationToken);
    }

    public Task<IEnumerable<Modelo>> ObtenerModelosAsync(CancellationToken cancellationToken = default)
    {
        return _repositorioCatalogos.ObtenerModelosAsync(cancellationToken);
    }

    public Task<IEnumerable<Ubicacion>> ObtenerUbicacionesAsync(CancellationToken cancellationToken = default)
    {
        return _repositorioCatalogos.ObtenerUbicacionesAsync(cancellationToken);
    }

    public Task<IEnumerable<Especialidad>> ObtenerEspecialidadesAsync(CancellationToken cancellationToken = default)
    {
        return _repositorioCatalogos.ObtenerEspecialidadesAsync(cancellationToken);
    }

    public Task<IEnumerable<TipoOrden>> ObtenerTiposOrdenAsync(CancellationToken cancellationToken = default)
    {
        return _repositorioCatalogos.ObtenerTiposOrdenAsync(cancellationToken);
    }

    public Task<IEnumerable<TipoGravedad>> ObtenerTiposGravedadAsync(CancellationToken cancellationToken = default)
    {
        return _repositorioCatalogos.ObtenerTiposGravedadAsync(cancellationToken);
    }
}
