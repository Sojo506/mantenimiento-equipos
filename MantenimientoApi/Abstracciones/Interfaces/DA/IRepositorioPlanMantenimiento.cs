using Abstracciones.Entidades;

namespace Abstracciones.Interfaces.DA;

/// <summary>
/// Contrato de acceso a datos para planes de mantenimiento preventivo.
/// </summary>
public interface IRepositorioPlanMantenimiento
{
    Task<IEnumerable<PlanDeMantenimiento>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PlanDeMantenimiento>> ObtenerPlanesPendientesAsync(DateTime fechaCorte, CancellationToken cancellationToken = default);
    Task<PlanDeMantenimiento?> ObtenerPorIdAsync(int idPlan, CancellationToken cancellationToken = default);
    Task<PlanDeMantenimiento?> ObtenerPorEquipoAsync(string codigoEquipo, CancellationToken cancellationToken = default);
    Task AgregarAsync(PlanDeMantenimiento plan, CancellationToken cancellationToken = default);
    Task ActualizarAsync(PlanDeMantenimiento plan, CancellationToken cancellationToken = default);
    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
