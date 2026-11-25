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

    public async Task<IEnumerable<ReporteOrdenesEstadoDto>> ObtenerReporteOrdenesAsync(
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.OrdenesDeTrabajo
            .Include(o => o.Equipo)
            .Include(o => o.TipoOrden)
            .Include(o => o.Estado)
            .Include(o => o.Tecnico)
            .Include(o => o.RepuestosUsados)
            .AsNoTracking()
            .AsQueryable();

        if (fechaInicio.HasValue)
            query = query.Where(o => o.FechaCreacion >= fechaInicio.Value);

        if (fechaFin.HasValue)
            query = query.Where(o => o.FechaCreacion <= fechaFin.Value);

        var ordenes = await query.OrderByDescending(o => o.FechaCreacion).ToListAsync(cancellationToken);

        return ordenes.Select(o => new ReporteOrdenesEstadoDto
        {
            NumeroOrden = o.Numero,
            CodigoEquipo = o.CodigoEquipo,
            EquipoNombre = o.Equipo?.Nombre ?? string.Empty,
            TipoOrden = o.TipoOrden?.Nombre ?? string.Empty,
            Prioridad = o.Prioridad,
            Estado = o.Estado?.Nombre ?? string.Empty,
            TecnicoAsignado = o.Tecnico != null ? $"{o.Tecnico.Nombre} {o.Tecnico.PrimerApellido}".Trim() : "Sin asignar",
            FechaCreacion = o.FechaCreacion,
            FechaLimite = o.FechaLimite,
            TiempoInvertidoHoras = o.TiempoInvertido,
            CostoRepuestos = o.RepuestosUsados.Sum(r => r.CantidadUsada * r.CostoAplicado)
        });
    }

    public async Task<IEnumerable<ReporteEquipoCriticoDto>> ObtenerReporteEquiposCriticosAsync(
        CancellationToken cancellationToken = default)
    {
        var equipos = await _context.Equipos
            .Include(e => e.TipoEquipo)
            .Include(e => e.Ubicacion)
            .Include(e => e.Estado)
            .Include(e => e.FallasRegistradas)
            .Include(e => e.OrdenesDeTrabajo)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return equipos
            .OrderByDescending(e => e.FallasRegistradas.Count)
            .ThenBy(e => e.CodigoEquipo)
            .Select(e => new ReporteEquipoCriticoDto
            {
                CodigoEquipo = e.CodigoEquipo,
                NombreEquipo = e.Nombre,
                TipoEquipo = e.TipoEquipo?.Nombre ?? string.Empty,
                Ubicacion = e.Ubicacion?.Nombre ?? string.Empty,
                EstadoActual = e.Estado?.Nombre ?? string.Empty,
                TotalFallasRegistradas = e.FallasRegistradas.Count,
                OrdenesAbiertas = e.OrdenesDeTrabajo.Count(o => o.IdEstado != 11),
                OrdenesCerradas = e.OrdenesDeTrabajo.Count(o => o.IdEstado == 11),
                HorasMantenimientoTotal = e.OrdenesDeTrabajo.Sum(o => o.TiempoInvertido)
            });
    }

    public async Task<IEnumerable<ReporteProductividadTecnicoDto>> ObtenerReporteProductividadTecnicosAsync(
        CancellationToken cancellationToken = default)
    {
        var tecnicos = await _context.Usuarios
            .Where(u => u.IdTipoUsuario == 3)
            .Include(u => u.Especialidad)
            .Include(u => u.OrdenesAsignadas)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return tecnicos
            .OrderByDescending(t => t.OrdenesAsignadas.Count(o => o.IdEstado == 11))
            .Select(t => new ReporteProductividadTecnicoDto
            {
                Identificacion = t.Identificacion,
                NombreTecnico = $"{t.Nombre} {t.PrimerApellido} {t.SegundoApellido}".Trim(),
                Especialidad = t.Especialidad?.Nombre ?? "General",
                Disponible = t.Disponibilidad && t.IdEstado == 1,
                OrdenesEnProceso = t.OrdenesAsignadas.Count(o => o.IdEstado == 9),
                OrdenesAsignadas = t.OrdenesAsignadas.Count(o => o.IdEstado == 8),
                OrdenesCerradas = t.OrdenesAsignadas.Count(o => o.IdEstado == 11),
                HorasTotalesRegistradas = t.OrdenesAsignadas.Sum(o => o.TiempoInvertido)
            });
    }
}
