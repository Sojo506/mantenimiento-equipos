using Abstracciones.Entidades;

namespace Abstracciones.Interfaces.DA;

/// <summary>
/// Contrato de acceso a datos para la bitácora de órdenes.
/// Esta bitácora es estrictamente inmutable: no contiene métodos de actualización ni de eliminación.
/// </summary>
public interface IRepositorioBitacoraOrden
{
    Task<IEnumerable<BitacoraOrden>> ObtenerHistorialPorOrdenAsync(int numeroOrden, CancellationToken cancellationToken = default);
    Task RegistrarAsync(BitacoraOrden entrada, CancellationToken cancellationToken = default);
    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
