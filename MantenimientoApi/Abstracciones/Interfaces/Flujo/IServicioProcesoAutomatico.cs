using Abstracciones.DTOs.Planes;

namespace Abstracciones.Interfaces.Flujo;

public interface IServicioProcesoAutomatico
{
    Task<ResultadoProcesoAutomaticoDto> EjecutarAsync(CancellationToken cancellationToken = default);
}
