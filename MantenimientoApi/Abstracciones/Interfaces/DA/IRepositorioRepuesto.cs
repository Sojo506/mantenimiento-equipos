using Abstracciones.Entidades;

namespace Abstracciones.Interfaces.DA;

/// <summary>
/// Contrato de acceso a datos para la entidad Repuesto con soporte de concurrencia optimista.
/// </summary>
public interface IRepositorioRepuesto
{
    Task<IEnumerable<Repuesto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Repuesto>> ObtenerBajoStockMinimoAsync(CancellationToken cancellationToken = default);
    Task<Repuesto?> ObtenerPorCodigoAsync(string codigoRepuesto, CancellationToken cancellationToken = default);
    Task<bool> ExisteCodigoAsync(string codigoRepuesto, CancellationToken cancellationToken = default);
    Task AgregarAsync(Repuesto repuesto, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Repuesto repuesto, CancellationToken cancellationToken = default);
    Task EliminarAsync(string codigoRepuesto, CancellationToken cancellationToken = default);
    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
