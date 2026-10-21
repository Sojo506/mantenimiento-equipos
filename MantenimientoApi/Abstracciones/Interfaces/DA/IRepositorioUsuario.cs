using Abstracciones.Entidades;

namespace Abstracciones.Interfaces.DA;

/// <summary>
/// Contrato de acceso a datos para la entidad Usuario.
/// </summary>
public interface IRepositorioUsuario
{
    Task<IEnumerable<Usuario>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<Usuario?> ObtenerPorIdentificacionAsync(string identificacion, CancellationToken cancellationToken = default);
    Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken cancellationToken = default);
    Task<IEnumerable<Usuario>> ObtenerTecnicosDisponiblesAsync(CancellationToken cancellationToken = default);
    Task AgregarAsync(Usuario usuario, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Usuario usuario, CancellationToken cancellationToken = default);
    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
