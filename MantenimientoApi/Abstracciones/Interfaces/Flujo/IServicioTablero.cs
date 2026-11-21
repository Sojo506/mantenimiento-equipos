using Abstracciones.DTOs.Tablero;

namespace Abstracciones.Interfaces.Flujo;

public interface IServicioTablero
{
    Task<IEnumerable<EquipoPorEstadoDto>> ObtenerEquiposPorEstadoAsync(
        CancellationToken cancellationToken = default);

    Task<IEnumerable<OrdenPorPrioridadDto>> ObtenerOrdenesPorPrioridadAsync(
        CancellationToken cancellationToken = default);

    Task<IEnumerable<RepuestosBajoMinimoDto>> ObtenerRepuestosBajoMinimoAsync(
        CancellationToken cancellationToken = default);
}