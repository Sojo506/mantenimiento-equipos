using Web_Mantenimiento.Models.Tablero;
using Web_Mantenimiento.Services.Base;

namespace Web_Mantenimiento.Services;

public class ApiTableroService : ApiClientBase
{
    public ApiTableroService(IHttpClientFactory factory) : base(factory)
    {
    }

    public Task<(List<EquipoPorEstadoViewModel>? Datos, string? Error)> ObtenerEquiposPorEstadoAsync(
        CancellationToken ct = default) =>
        GetAsync<List<EquipoPorEstadoViewModel>>("api/tablero/equipos-por-estado", ct);

    public Task<(List<OrdenPorPrioridadViewModel>? Datos, string? Error)> ObtenerOrdenesPorPrioridadAsync(
        CancellationToken ct = default) =>
        GetAsync<List<OrdenPorPrioridadViewModel>>("api/tablero/ordenes-por-prioridad", ct);

    public Task<(RepuestosBajoMinimoResumenViewModel? Datos, string? Error)> ObtenerRepuestosBajoMinimoAsync(
        CancellationToken ct = default) =>
        GetAsync<RepuestosBajoMinimoResumenViewModel>("api/tablero/repuestos-bajo-minimo", ct);

    public async Task<TableroResumenViewModel> ObtenerResumenAsync(CancellationToken ct = default)
    {
        var tEquipos = ObtenerEquiposPorEstadoAsync(ct);
        var tOrdenes = ObtenerOrdenesPorPrioridadAsync(ct);
        var tRepuestos = ObtenerRepuestosBajoMinimoAsync(ct);

        await Task.WhenAll(tEquipos, tOrdenes, tRepuestos);

        var (equipos, errEq) = tEquipos.Result;
        var (ordenes, errOrd) = tOrdenes.Result;
        var (repuestos, errRep) = tRepuestos.Result;

        return new TableroResumenViewModel
        {
            EquiposPorEstado = equipos ?? new(),
            OrdenesPorPrioridad = ordenes ?? new(),
            RepuestosBajoMinimo = repuestos ?? new(),
            Error = errEq ?? errOrd ?? errRep
        };
    }
}
