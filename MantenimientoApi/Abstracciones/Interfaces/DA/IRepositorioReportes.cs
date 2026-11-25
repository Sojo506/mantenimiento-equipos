using Abstracciones.DTOs.Reportes;

namespace Abstracciones.Interfaces.DA;

/// <summary>
/// Contrato de acceso a datos para reportes y procedimientos almacenados en la base de datos.
/// </summary>
public interface IRepositorioReportes
{
    Task<IEnumerable<ReporteConsumoRepuestoDto>> ObtenerConsumoRepuestosPorPeriodoAsync(
        DateTime fechaInicio, 
        DateTime fechaFin, 
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ReporteOrdenesEstadoDto>> ObtenerReporteOrdenesAsync(
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ReporteEquipoCriticoDto>> ObtenerReporteEquiposCriticosAsync(
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ReporteProductividadTecnicoDto>> ObtenerReporteProductividadTecnicosAsync(
        CancellationToken cancellationToken = default);
}
