using Abstracciones.DTOs.Tablero;
using Abstracciones.Interfaces.DA;
using Microsoft.EntityFrameworkCore;

namespace DA.Repositorios;

public class RepositorioTablero : IRepositorioTablero
{
    private readonly MantenimientoDbContext _context;

    public RepositorioTablero(MantenimientoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EquipoPorEstadoDto>>
        ObtenerEquiposPorEstadoAsync(
            CancellationToken cancellationToken = default)
    {
        return await _context.Equipos
            .AsNoTracking()
            .GroupBy(e => new
            {
                e.IdEstado,
                Estado = e.Estado!.Nombre
            })
            .Select(g => new EquipoPorEstadoDto
            {
                IdEstado = g.Key.IdEstado,
                Estado = g.Key.Estado,
                Cantidad = g.Count()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OrdenPorPrioridadDto>>
        ObtenerOrdenesPorPrioridadAsync(
            CancellationToken cancellationToken = default)
    {
        return await _context.OrdenesDeTrabajo
            .AsNoTracking()
            .GroupBy(o => o.Prioridad)
            .Select(g => new OrdenPorPrioridadDto
            {
                Prioridad = g.Key,
                Total = g.Count(),
                Activas = g.Count(o =>
                    o.IdEstado != 11 && o.IdEstado != 10)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RepuestosBajoMinimoDto>>
        ObtenerRepuestosBajoMinimoAsync(
            CancellationToken cancellationToken = default)
    {
        return await _context.Repuestos
            .AsNoTracking()
            .Where(r => r.Stock <= r.StockMinimo)
            .Select(r => new RepuestosBajoMinimoDto
            {
                CodigoRepuesto = r.CodigoRepuesto,
                Nombre = r.Nombre,
                Stock = r.Stock,
                StockMinimo = r.StockMinimo,
                Deficit = r.StockMinimo - r.Stock,
                CostoUnitario = r.CostoUnitario
            })
            .ToListAsync(cancellationToken);
    }
}