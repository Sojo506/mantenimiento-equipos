using Abstracciones.Entidades;

namespace Abstracciones.Interfaces.DA;

/// <summary>
/// Contrato de acceso a datos para la entidad OrdenDeTrabajo.
/// </summary>
public interface IRepositorioOrdenDeTrabajo
{
    Task<IEnumerable<OrdenDeTrabajo>> ObtenerTodasAsync(CancellationToken cancellationToken = default);
    Task<OrdenDeTrabajo?> ObtenerPorNumeroAsync(int numero, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrdenDeTrabajo>> ObtenerPorEquipoAsync(string codigoEquipo, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrdenDeTrabajo>> ObtenerPorTecnicoAsync(string identificacionTecnico, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrdenDeTrabajo>> ObtenerPorSolicitanteAsync(string identificacionSolicitante, CancellationToken cancellationToken = default);
    Task<bool> TieneOrdenCorrectivaAbiertaAsync(string codigoEquipo, CancellationToken cancellationToken = default);
    Task<int> ContarOrdenesEnProcesoPorTecnicoAsync(string identificacionTecnico, CancellationToken cancellationToken = default);
    Task AgregarAsync(OrdenDeTrabajo orden, CancellationToken cancellationToken = default);
    Task ActualizarAsync(OrdenDeTrabajo orden, CancellationToken cancellationToken = default);
    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
