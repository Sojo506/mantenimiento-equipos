using Abstracciones.Entidades;
using Abstracciones.Interfaces.DA;
using Microsoft.EntityFrameworkCore;

namespace DA.Repositorios;

/// <summary>
/// Implementación del repositorio de acceso a datos para OrdenDeTrabajo.
/// </summary>
public class RepositorioOrdenDeTrabajo : IRepositorioOrdenDeTrabajo
{
    private readonly MantenimientoDbContext _context;

    public RepositorioOrdenDeTrabajo(MantenimientoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrdenDeTrabajo>> ObtenerTodasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OrdenesDeTrabajo
            .Include(o => o.Equipo)
            .Include(o => o.Solicitante)
            .Include(o => o.Tecnico)
            .Include(o => o.TipoOrden)
            .Include(o => o.Estado)
            .Include(o => o.RepuestosUsados)
                .ThenInclude(ru => ru.Repuesto)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<OrdenDeTrabajo?> ObtenerPorNumeroAsync(int numero, CancellationToken cancellationToken = default)
    {
        return await _context.OrdenesDeTrabajo
            .Include(o => o.Equipo)
            .Include(o => o.Solicitante)
            .Include(o => o.Tecnico)
            .Include(o => o.TipoOrden)
            .Include(o => o.Estado)
            .Include(o => o.RepuestosUsados)
                .ThenInclude(ru => ru.Repuesto)
            .Include(o => o.FallasRegistradas)
            .Include(o => o.Bitacoras.OrderBy(b => b.Fecha))
            .FirstOrDefaultAsync(o => o.Numero == numero, cancellationToken);
    }

    public async Task<IEnumerable<OrdenDeTrabajo>> ObtenerPorEquipoAsync(string codigoEquipo, CancellationToken cancellationToken = default)
    {
        return await _context.OrdenesDeTrabajo
            .Where(o => o.CodigoEquipo == codigoEquipo)
            .Include(o => o.TipoOrden)
            .Include(o => o.Estado)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OrdenDeTrabajo>> ObtenerPorTecnicoAsync(string identificacionTecnico, CancellationToken cancellationToken = default)
    {
        return await _context.OrdenesDeTrabajo
            .Where(o => o.IdentificacionTecnico == identificacionTecnico)
            .Include(o => o.Equipo)
            .Include(o => o.TipoOrden)
            .Include(o => o.Estado)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OrdenDeTrabajo>> ObtenerPorSolicitanteAsync(string identificacionSolicitante, CancellationToken cancellationToken = default)
    {
        return await _context.OrdenesDeTrabajo
            .Where(o => o.IdentificacionSolicitante == identificacionSolicitante)
            .Include(o => o.Equipo)
            .Include(o => o.TipoOrden)
            .Include(o => o.Estado)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> TieneOrdenCorrectivaAbiertaAsync(string codigoEquipo, CancellationToken cancellationToken = default)
    {
        // Estados no cerrados (7: Abierta, 8: Asignada, 9: En Proceso, 10: En Espera de Repuesto, 12: Escalada)
        return await _context.OrdenesDeTrabajo
            .AnyAsync(o => o.CodigoEquipo == codigoEquipo && 
                           o.IdTipoOrden == 2 && 
                           o.IdEstado != 11, cancellationToken);
    }

    public async Task<int> ContarOrdenesEnProcesoPorTecnicoAsync(string identificacionTecnico, CancellationToken cancellationToken = default)
    {
        // Estado 9 = En Proceso
        return await _context.OrdenesDeTrabajo
            .CountAsync(o => o.IdentificacionTecnico == identificacionTecnico && o.IdEstado == 9, cancellationToken);
    }

    public async Task AgregarAsync(OrdenDeTrabajo orden, CancellationToken cancellationToken = default)
    {
        await _context.OrdenesDeTrabajo.AddAsync(orden, cancellationToken);
    }

    public Task ActualizarAsync(OrdenDeTrabajo orden, CancellationToken cancellationToken = default)
    {
        _context.OrdenesDeTrabajo.Update(orden);
        return Task.CompletedTask;
    }

    public async Task GuardarCambiosAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
