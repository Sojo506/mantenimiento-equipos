using Abstracciones.Entidades;
using Abstracciones.Interfaces.DA;
using Microsoft.EntityFrameworkCore;

namespace DA.Repositorios;

/// <summary>
/// Repositorio para la consulta ágil de catálogos generales del sistema.
/// </summary>
public class RepositorioCatalogos : IRepositorioCatalogos
{
    private readonly MantenimientoDbContext _context;

    public RepositorioCatalogos(MantenimientoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Estado>> ObtenerEstadosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Estados.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TipoUsuario>> ObtenerTiposUsuarioAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TiposUsuario.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TipoEquipo>> ObtenerTiposEquipoAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TiposEquipo.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Marca>> ObtenerMarcasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Marcas.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Modelo>> ObtenerModelosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Modelos.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Ubicacion>> ObtenerUbicacionesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Ubicaciones.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Especialidad>> ObtenerEspecialidadesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Especialidades.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TipoOrden>> ObtenerTiposOrdenAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TiposOrden.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TipoGravedad>> ObtenerTiposGravedadAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TiposGravedad.AsNoTracking().ToListAsync(cancellationToken);
    }
}
