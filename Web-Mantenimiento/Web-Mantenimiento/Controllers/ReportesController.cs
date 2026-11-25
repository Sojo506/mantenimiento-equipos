using Microsoft.AspNetCore.Mvc;
using Web_Mantenimiento.Filtros;
using Web_Mantenimiento.Models.Reportes;
using Web_Mantenimiento.Services;

namespace Web_Mantenimiento.Controllers;

[AutorizacionSesion("Administrador", "Supervisor")]
public class ReportesController : Controller
{
    private readonly ApiReporteService _reporteService;

    public ReportesController(ApiReporteService reporteService)
    {
        _reporteService = reporteService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        DateTime? fechaInicio,
        DateTime? fechaFin,
        CancellationToken ct = default)
    {
        var inicio = fechaInicio ?? new DateTime(2026, 1, 1);
        var fin = fechaFin ?? DateTime.Today.AddDays(1);

        var (consumo, error) = await _reporteService.ObtenerConsumoRepuestosAsync(inicio, fin, ct);
        if (error != null)
            ViewBag.Error = error;

        var vm = new ReportesFiltroViewModel
        {
            FechaInicio = inicio,
            FechaFin = fin,
            ConsumoRepuestos = consumo ?? new List<ReporteConsumoRepuestoViewModel>()
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> DescargarCsv(
        string tipo,
        DateTime? fechaInicio,
        DateTime? fechaFin,
        CancellationToken ct = default)
    {
        var (bytes, nombreArchivo, error) = await _reporteService.DescargarCsvAsync(tipo, fechaInicio, fechaFin, ct);
        if (error != null || bytes == null)
        {
            TempData["Error"] = error ?? "No se pudo generar el archivo CSV.";
            return RedirectToAction(nameof(Index), new { fechaInicio, fechaFin });
        }

        return File(bytes, "text/csv; charset=utf-8", nombreArchivo ?? "reporte.csv");
    }
}
