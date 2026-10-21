using Abstracciones.Entidades;
using Abstracciones.Interfaces.DA;
using Microsoft.EntityFrameworkCore;

namespace DA.Repositorios;

/// <summary>
/// Implementación del repositorio de acceso a datos para Repuesto con soporte de concurrencia optimista.
/// </summary>
public class RepositorioRepuesto : IRepositorioRepuesto
{
    private readonly MantenimientoDbContext _context;

    public RepositorioRepuesto(MantenimientoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Repuesto>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Repuestos
            .Include(r => r.Estado)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Repuesto>> ObtenerBajoStockMinimoAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Repuestos
            .Include(r => r.Estado)
            .Where(r => r.Stock <= r.StockMinimo)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Repuesto?> ObtenerPorCodigoAsync(string codigoRepuesto, CancellationToken cancellationToken = default)
    {
        return await _context.Repuestos
            .Include(r => r.Estado)
            .FirstOrDefaultAsync(r => r.CodigoRepuesto == codigoRepuesto, cancellationToken);
    }

    public async Task<bool> ExisteCodigoAsync(string codigoRepuesto, CancellationToken cancellationToken = default)
    {
        return await _context.Repuestos
            .AnyAsync(r => r.CodigoRepuesto == codigoRepuesto, cancellationToken);
    }

    public async Task AgregarAsync(Repuesto repuesto, CancellationToken cancellationToken = default)
    {
        await _context.Repuestos.AddAsync(repuesto, cancellationToken);
    }

    public Task ActualizarAsync(Repuesto repuesto, CancellationToken cancellationToken = default)
    {
        _context.Repuestos.Update(repuesto);
        return Task.CompletedTask;
    }

    public async Task EliminarAsync(string codigoRepuesto, CancellationToken cancellationToken = default)
    {
        var repuesto = await _context.Repuestos.FindAsync(new object[] { codigoRepuesto }, cancellationToken);
        if (repuesto != null)
        {
            _context.Repuestos.Remove(repuesto);
        }
    }

    public async Task GuardarCambiosAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
