using Microsoft.AspNetCore.Mvc;
using Web_Mantenimiento.Filtros;
using Web_Mantenimiento.Models.Fallas;
using Web_Mantenimiento.Services;

namespace Web_Mantenimiento.Controllers;

[AutorizacionSesion]
public class FallasController : Controller
{
    private readonly ApiFallaService _fallaService;

    public FallasController(ApiFallaService fallaService)
    {
        _fallaService = fallaService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? codigoEquipo, CancellationToken ct = default)
    {
        var (fallas, error) = await _fallaService.ObtenerTodasAsync(codigoEquipo, ct);
        if (error != null)
            ViewBag.Error = error;

        ViewData["CodigoEquipoFiltro"] = codigoEquipo;
        return View(fallas ?? new List<FallaViewModel>());
    }

    [HttpGet]
    public IActionResult Crear(string? codigoEquipo)
    {
        return View(new RegistrarFallaViewModel
        {
            CodigoEquipo = codigoEquipo ?? string.Empty
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(RegistrarFallaViewModel modelo, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var idUsuario = HttpContext.Session.GetString("Identificacion") ?? "USR-005";
        var (creada, error) = await _fallaService.RegistrarAsync(modelo, idUsuario, ct);
        if (error != null || creada == null)
        {
            ViewBag.Error = error ?? "No se pudo registrar la falla.";
            return View(modelo);
        }

        if (creada.PasoAEnRiesgoAutomatico)
        {
            TempData["Mensaje"] = $"Falla #{creada.Id} registrada (Orden #{creada.NumeroOrden}). Alerta: El equipo {creada.CodigoEquipo} pasó automáticamente al estado 'En Riesgo' por reincidencia de fallas.";
        }
        else
        {
            TempData["Mensaje"] = $"Falla #{creada.Id} registrada correctamente y vinculada a la Orden #{creada.NumeroOrden}.";
        }

        return RedirectToAction(nameof(Index), new { codigoEquipo = creada.CodigoEquipo });
    }
}
