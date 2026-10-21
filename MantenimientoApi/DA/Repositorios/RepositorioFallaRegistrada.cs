using Abstracciones.Entidades;
using Abstracciones.Interfaces.DA;
using Microsoft.EntityFrameworkCore;

namespace DA.Repositorios;

/// <summary>
/// Implementación del repositorio de acceso a datos para fallas registradas con soporte para la regla RN-03.
/// </summary>
public class RepositorioFallaRegistrada : IRepositorioFallaRegistrada
{
    private readonly MantenimientoDbContext _context;

    public RepositorioFallaRegistrada(MantenimientoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FallaRegistrada>> ObtenerPorEquipoAsync(string codigoEquipo, CancellationToken cancellationToken = default)
    {
        return await _context.FallasRegistradas
            .Where(f => f.CodigoEquipo == codigoEquipo)
            .Include(f => f.TipoGravedad)
            .OrderByDescending(f => f.Fecha)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<int> ContarFallasUltimosMesesAsync(string codigoEquipo, int meses, CancellationToken cancellationToken = default)
    {
        DateTime fechaLimite = DateTime.UtcNow.AddMonths(-meses);
        return await _context.FallasRegistradas
            .CountAsync(f => f.CodigoEquipo == codigoEquipo && f.Fecha >= fechaLimite, cancellationToken);
    }

    public async Task AgregarAsync(FallaRegistrada falla, CancellationToken cancellationToken = default)
    {
        await _context.FallasRegistradas.AddAsync(falla, cancellationToken);
    }

    public async Task GuardarCambiosAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
