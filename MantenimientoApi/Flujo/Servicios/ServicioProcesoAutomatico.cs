using Abstracciones.DTOs.Planes;
using Abstracciones.Interfaces.Flujo;

namespace Flujo.Servicios;

public class ServicioProcesoAutomatico : IServicioProcesoAutomatico
{
    private readonly IServicioOrdenDeTrabajo _servicioOrden;
    private readonly IServicioPlanMantenimiento _servicioPlan;

    public ServicioProcesoAutomatico(IServicioOrdenDeTrabajo servicioOrden, IServicioPlanMantenimiento servicioPlan)
    {
        _servicioOrden = servicioOrden;
        _servicioPlan = servicioPlan;
    }

    public async Task<ResultadoProcesoAutomaticoDto> EjecutarAsync(CancellationToken cancellationToken = default)
    {
        var escaladas = await _servicioOrden.EscalarCriticasSinAtenderAsync(cancellationToken);
        var generadas = await _servicioPlan.GenerarOrdenesPreventivasAsync(cancellationToken);

        return new ResultadoProcesoAutomaticoDto
        {
            OrdenesEscaladas = escaladas.Count,
            OrdenesPreventivasGeneradas = generadas.Count,
            NumerosOrdenesEscaladas = escaladas.ToList(),
            NumerosOrdenesGeneradas = generadas.ToList()
        };
    }
}
