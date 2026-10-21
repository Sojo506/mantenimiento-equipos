using Abstracciones.DTOs.Reportes;
using Abstracciones.Interfaces.DA;
using Microsoft.EntityFrameworkCore;

namespace DA.Repositorios;

/// <summary>
/// Repositorio para la ejecución de procedimientos almacenados y consultas analíticas de reportes.
/// </summary>
public class RepositorioReportes : IRepositorioReportes
{
    private readonly MantenimientoDbContext _context;

    public RepositorioReportes(MantenimientoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReporteConsumoRepuestoDto>> ObtenerConsumoRepuestosPorPeriodoAsync(
        DateTime fechaInicio, 
        DateTime fechaFin, 
        CancellationToken cancellationToken = default)
    {
        return await _context.ReporteConsumoRepuestos
            .FromSqlInterpolated($"EXEC sp_ReporteConsumoRepuestosPorPeriodo @FechaInicio = {fechaInicio}, @FechaFin = {fechaFin}")
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
