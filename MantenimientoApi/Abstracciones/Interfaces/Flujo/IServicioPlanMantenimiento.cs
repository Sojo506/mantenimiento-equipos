using Abstracciones.DTOs.Planes;

namespace Abstracciones.Interfaces.Flujo;

public interface IServicioPlanMantenimiento
{
    Task<IEnumerable<PlanMantenimientoDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<PlanMantenimientoDto> ObtenerPorIdAsync(int idPlan, CancellationToken cancellationToken = default);
    Task<PlanMantenimientoDto> CrearAsync(CrearPlanMantenimientoDto dto, CancellationToken cancellationToken = default);
    Task<PlanMantenimientoDto> ActualizarAsync(int idPlan, ActualizarPlanMantenimientoDto dto, CancellationToken cancellationToken = default);
    Task EliminarAsync(int idPlan, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> GenerarOrdenesPreventivasAsync(CancellationToken cancellationToken = default);
}
