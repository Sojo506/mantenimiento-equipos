using Abstracciones.Entidades;
using Abstracciones.Interfaces.DA;
using Microsoft.EntityFrameworkCore;

namespace DA.Repositorios;

/// <summary>
/// Repositorio inmutable para la trazabilidad y auditoría de órdenes de trabajo.
/// Por diseño arquitectural y requerimiento estricto de negocio, no expone métodos de modificación ni eliminación.
/// </summary>
public class RepositorioBitacoraOrden : IRepositorioBitacoraOrden
{
    private readonly MantenimientoDbContext _context;

    public RepositorioBitacoraOrden(MantenimientoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BitacoraOrden>> ObtenerHistorialPorOrdenAsync(int numeroOrden, CancellationToken cancellationToken = default)
    {
        return await _context.BitacorasOrden
            .Where(b => b.Numero == numeroOrden)
            .Include(b => b.Usuario)
            .OrderBy(b => b.Fecha)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task RegistrarAsync(BitacoraOrden entrada, CancellationToken cancellationToken = default)
    {
        await _context.BitacorasOrden.AddAsync(entrada, cancellationToken);
    }

    public async Task GuardarCambiosAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
