using Abstracciones.Entidades;

namespace Abstracciones.Interfaces.DA;

/// <summary>
/// Contrato de acceso a datos para la entidad Equipo.
/// </summary>
public interface IRepositorioEquipo
{
    Task<IEnumerable<Equipo>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<Equipo?> ObtenerPorCodigoAsync(string codigoEquipo, CancellationToken cancellationToken = default);
    Task<bool> ExisteCodigoAsync(string codigoEquipo, CancellationToken cancellationToken = default);
    Task AgregarAsync(Equipo equipo, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Equipo equipo, CancellationToken cancellationToken = default);
    Task EliminarAsync(string codigoEquipo, CancellationToken cancellationToken = default);
    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
