using Abstracciones.Entidades;
using Abstracciones.Interfaces.DA;
using Microsoft.EntityFrameworkCore;

namespace DA.Repositorios;

/// <summary>
/// Implementación del repositorio de acceso a datos para la entidad Equipo.
/// </summary>
public class RepositorioEquipo : IRepositorioEquipo
{
    private readonly MantenimientoDbContext _context;

    public RepositorioEquipo(MantenimientoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Equipo>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Equipos
            .Include(e => e.TipoEquipo)
            .Include(e => e.Marca)
            .Include(e => e.Modelo)
            .Include(e => e.Ubicacion)
            .Include(e => e.Estado)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Equipo?> ObtenerPorCodigoAsync(string codigoEquipo, CancellationToken cancellationToken = default)
    {
        return await _context.Equipos
            .Include(e => e.TipoEquipo)
            .Include(e => e.Marca)
            .Include(e => e.Modelo)
            .Include(e => e.Ubicacion)
            .Include(e => e.Estado)
            .FirstOrDefaultAsync(e => e.CodigoEquipo == codigoEquipo, cancellationToken);
    }

    public async Task<bool> ExisteCodigoAsync(string codigoEquipo, CancellationToken cancellationToken = default)
    {
        return await _context.Equipos
            .AnyAsync(e => e.CodigoEquipo == codigoEquipo, cancellationToken);
    }

    public async Task AgregarAsync(Equipo equipo, CancellationToken cancellationToken = default)
    {
        await _context.Equipos.AddAsync(equipo, cancellationToken);
    }

    public Task ActualizarAsync(Equipo equipo, CancellationToken cancellationToken = default)
    {
        _context.Equipos.Update(equipo);
        return Task.CompletedTask;
    }

    public async Task EliminarAsync(string codigoEquipo, CancellationToken cancellationToken = default)
    {
        var equipo = await _context.Equipos.FindAsync(new object[] { codigoEquipo }, cancellationToken);
        if (equipo != null)
        {
            _context.Equipos.Remove(equipo);
        }
    }

    public async Task GuardarCambiosAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
