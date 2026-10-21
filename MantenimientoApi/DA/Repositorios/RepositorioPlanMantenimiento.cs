using Abstracciones.Entidades;
using Abstracciones.Interfaces.DA;
using Microsoft.EntityFrameworkCore;

namespace DA.Repositorios;

/// <summary>
/// Implementación del repositorio de acceso a datos para planes de mantenimiento preventivo.
/// </summary>
public class RepositorioPlanMantenimiento : IRepositorioPlanMantenimiento
{
    private readonly MantenimientoDbContext _context;

    public RepositorioPlanMantenimiento(MantenimientoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PlanDeMantenimiento>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PlanesDeMantenimiento
            .Include(p => p.Equipo)
            .Include(p => p.Estado)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PlanDeMantenimiento>> ObtenerPlanesPendientesAsync(DateTime fechaCorte, CancellationToken cancellationToken = default)
    {
        return await _context.PlanesDeMantenimiento
            .Where(p => p.IdEstado == 1 && p.ProximaFecha <= fechaCorte)
            .Include(p => p.Equipo)
            .ToListAsync(cancellationToken);
    }

    public async Task<PlanDeMantenimiento?> ObtenerPorIdAsync(int idPlan, CancellationToken cancellationToken = default)
    {
        return await _context.PlanesDeMantenimiento
            .Include(p => p.Equipo)
            .Include(p => p.Estado)
            .FirstOrDefaultAsync(p => p.IdPlanMantenimiento == idPlan, cancellationToken);
    }

    public async Task<PlanDeMantenimiento?> ObtenerPorEquipoAsync(string codigoEquipo, CancellationToken cancellationToken = default)
    {
        return await _context.PlanesDeMantenimiento
            .Include(p => p.Equipo)
            .Include(p => p.Estado)
            .FirstOrDefaultAsync(p => p.CodigoEquipo == codigoEquipo, cancellationToken);
    }

    public async Task AgregarAsync(PlanDeMantenimiento plan, CancellationToken cancellationToken = default)
    {
        await _context.PlanesDeMantenimiento.AddAsync(plan, cancellationToken);
    }

    public Task ActualizarAsync(PlanDeMantenimiento plan, CancellationToken cancellationToken = default)
    {
        _context.PlanesDeMantenimiento.Update(plan);
        return Task.CompletedTask;
    }

    public async Task GuardarCambiosAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
