using Abstracciones.DTOs.Ordenes;
using Abstracciones.Entidades;

namespace Abstracciones.Interfaces.Flujo;

/// <summary>
/// Contrato de la lógica de negocio para la gestión del ciclo de vida de órdenes de trabajo,
/// máquina de estados, trazabilidad en bitácora y consumo de repuestos.
/// </summary>
public interface IServicioOrdenDeTrabajo
{
    Task<IEnumerable<OrdenDeTrabajoDto>> ObtenerTodasAsync(CancellationToken cancellationToken = default);
    Task<OrdenDeTrabajoDto> ObtenerPorNumeroAsync(int numero, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrdenDeTrabajoDto>> ObtenerPorEquipoAsync(string codigoEquipo, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrdenDeTrabajoDto>> ObtenerPorTecnicoAsync(string identificacionTecnico, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrdenDeTrabajoDto>> ObtenerPorSolicitanteAsync(string identificacionSolicitante, CancellationToken cancellationToken = default);
    Task<OrdenDeTrabajoDto> CrearAsync(CrearOrdenDto dto, CancellationToken cancellationToken = default);
    Task<OrdenDeTrabajoDto> ActualizarAsync(int numero, ActualizarOrdenDto dto, CancellationToken cancellationToken = default);
    Task<OrdenDeTrabajoDto> CambiarEstadoAsync(int numero, CambiarEstadoOrdenDto dto, CancellationToken cancellationToken = default);
    Task<OrdenDeTrabajoDto> AsignarRepuestoAsync(int numero, AsignarRepuestoOrdenDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<BitacoraOrden>> ObtenerHistorialBitacoraAsync(int numero, CancellationToken cancellationToken = default);
}
