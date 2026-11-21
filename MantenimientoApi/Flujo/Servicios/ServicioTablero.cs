using Abstracciones.DTOs.Tablero;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;

namespace Flujo.Servicios;

public class ServicioTablero : IServicioTablero
{
    private readonly IRepositorioTablero _repositorioTablero;

    public ServicioTablero(IRepositorioTablero repositorioTablero)
    {
        _repositorioTablero = repositorioTablero;
    }

    public Task<IEnumerable<EquipoPorEstadoDto>>
        ObtenerEquiposPorEstadoAsync(
            CancellationToken cancellationToken = default)
    {
        return _repositorioTablero.ObtenerEquiposPorEstadoAsync(
            cancellationToken);
    }

    public Task<IEnumerable<OrdenPorPrioridadDto>>
        ObtenerOrdenesPorPrioridadAsync(
            CancellationToken cancellationToken = default)
    {
        return _repositorioTablero.ObtenerOrdenesPorPrioridadAsync(
            cancellationToken);
    }

    public Task<IEnumerable<RepuestosBajoMinimoDto>>
        ObtenerRepuestosBajoMinimoAsync(
            CancellationToken cancellationToken = default)
    {
        return _repositorioTablero.ObtenerRepuestosBajoMinimoAsync(
            cancellationToken);
    }
}