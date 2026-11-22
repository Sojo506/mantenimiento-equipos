using Microsoft.AspNetCore.Mvc;
using Web_Mantenimiento.Filtros;
using Web_Mantenimiento.Services;

namespace Web_Mantenimiento.Controllers;

[AutorizacionSesion]
public class TableroController : Controller
{
    private readonly ApiTableroService _tableroService;

    public TableroController(ApiTableroService tableroService)
    {
        _tableroService = tableroService;
    }

    [HttpGet]
    public async Task<JsonResult> MetricasJson(CancellationToken ct = default)
    {
        var resumen = await _tableroService.ObtenerResumenAsync(ct);
        return Json(new
        {
            totalEquipos = resumen.EquiposPorEstado.Sum(e => e.Cantidad),
            ordenesActivas = resumen.OrdenesPorPrioridad.Sum(o => o.Activas),
            repuestosCriticos = resumen.RepuestosBajoMinimo.TotalCriticos,
            resumen.EquiposPorEstado,
            resumen.OrdenesPorPrioridad,
            resumen.RepuestosBajoMinimo,
            resumen.Error
        });
    }

    [HttpGet]
    public async Task<IActionResult> ResumenParcial(CancellationToken ct = default)
    {
        var resumen = await _tableroService.ObtenerResumenAsync(ct);
        return PartialView("_ResumenTablero", resumen);
    }
}
