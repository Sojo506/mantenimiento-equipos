using Microsoft.AspNetCore.Mvc;
using Web_Mantenimiento.Filtros;
using Web_Mantenimiento.Services;

namespace Web_Mantenimiento.Controllers;

[AutorizacionSesion]
public class HomeController : Controller
{
    private readonly ApiTableroService _tableroService;

    public HomeController(ApiTableroService tableroService)
    {
        _tableroService = tableroService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var resumen = await _tableroService.ObtenerResumenAsync(ct);
        if (resumen.Error != null)
            ViewBag.Error = resumen.Error;

        return View(resumen);
    }
}
