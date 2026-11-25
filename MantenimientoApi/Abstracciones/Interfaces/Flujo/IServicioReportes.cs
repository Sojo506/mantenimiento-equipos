using Abstracciones.DTOs.Reportes;

namespace Abstracciones.Interfaces.Flujo;

/// <summary>
/// Contrato de la lógica de negocio para la generación de reportes analíticos y exportación a CSV.
/// </summary>
public interface IServicioReportes
{
    Task<IEnumerable<ReporteConsumoRepuestoDto>> ObtenerConsumoRepuestosAsync(DateTime fechaInicio, DateTime fechaFin, CancellationToken cancellationToken = default);
    Task<byte[]> GenerarCsvConsumoRepuestosAsync(DateTime fechaInicio, DateTime fechaFin, CancellationToken cancellationToken = default);

    Task<IEnumerable<ReporteOrdenesEstadoDto>> ObtenerReporteOrdenesAsync(DateTime? fechaInicio, DateTime? fechaFin, CancellationToken cancellationToken = default);
    Task<byte[]> GenerarCsvOrdenesAsync(DateTime? fechaInicio, DateTime? fechaFin, CancellationToken cancellationToken = default);

    Task<IEnumerable<ReporteEquipoCriticoDto>> ObtenerReporteEquiposCriticosAsync(CancellationToken cancellationToken = default);
    Task<byte[]> GenerarCsvEquiposCriticosAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<ReporteProductividadTecnicoDto>> ObtenerReporteProductividadTecnicosAsync(CancellationToken cancellationToken = default);
    Task<byte[]> GenerarCsvProductividadTecnicosAsync(CancellationToken cancellationToken = default);
}
