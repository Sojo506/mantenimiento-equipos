using Abstracciones.Entidades;

namespace Abstracciones.Interfaces.DA;

/// <summary>
/// Contrato de acceso a datos para fallas registradas.
/// </summary>
public interface IRepositorioFallaRegistrada
{
    Task<IEnumerable<FallaRegistrada>> ObtenerPorEquipoAsync(string codigoEquipo, CancellationToken cancellationToken = default);
    Task<int> ContarFallasUltimosMesesAsync(string codigoEquipo, int meses, CancellationToken cancellationToken = default);
    Task AgregarAsync(FallaRegistrada falla, CancellationToken cancellationToken = default);
    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
