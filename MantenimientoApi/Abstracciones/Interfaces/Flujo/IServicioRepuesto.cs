using Abstracciones.DTOs.Repuestos;

namespace Abstracciones.Interfaces.Flujo;

/// <summary>
/// Contrato de la lógica de negocio para la gestión de repuestos e inventario con concurrencia.
/// </summary>
public interface IServicioRepuesto
{
    Task<IEnumerable<RepuestoDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RepuestoDto>> ObtenerBajoStockMinimoAsync(CancellationToken cancellationToken = default);
    Task<RepuestoDto> ObtenerPorCodigoAsync(string codigoRepuesto, CancellationToken cancellationToken = default);
    Task<RepuestoDto> CrearAsync(CrearRepuestoDto dto, CancellationToken cancellationToken = default);
    Task<RepuestoDto> ActualizarAsync(string codigoRepuesto, ActualizarRepuestoDto dto, CancellationToken cancellationToken = default);
    Task EliminarAsync(string codigoRepuesto, CancellationToken cancellationToken = default);
}
