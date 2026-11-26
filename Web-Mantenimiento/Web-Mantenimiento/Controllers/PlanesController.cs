using Microsoft.AspNetCore.Mvc;
using Web_Mantenimiento.Filtros;
using Web_Mantenimiento.Models.Comun;
using Web_Mantenimiento.Models.Planes;
using Web_Mantenimiento.Services;

namespace Web_Mantenimiento.Controllers;

[AutorizacionSesion("Administrador", "Supervisor")]
public class PlanesController : Controller
{
    private readonly ApiPlanService _planService;

    public PlanesController(ApiPlanService planService)
    {
        _planService = planService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? busqueda, string? ordenarPor, int? idEstado, bool soloVencidos = false,
        int pagina = 1, CancellationToken ct = default)
    {
        var (resultado, error) = await _planService.ObtenerPaginadoAsync(
            busqueda, ordenarPor, idEstado, soloVencidos, pagina, 10, ct);

        if (error != null)
            ViewBag.Error = error;

        var filtros = new Dictionary<string, string>();
        if (idEstado.HasValue) filtros["idEstado"] = idEstado.Value.ToString();
        if (soloVencidos) filtros["soloVencidos"] = "true";

        return View(new PlanListadoViewModel
        {
            Planes = resultado?.Items ?? new List<PlanViewModel>(),
            Busqueda = busqueda,
            OrdenarPor = ordenarPor,
            IdEstado = idEstado,
            SoloVencidos = soloVencidos,
            Paginacion = new PaginacionViewModel
            {
                Meta = resultado?.Paginacion ?? new PaginacionMeta(),
                Busqueda = busqueda,
                OrdenarPor = ordenarPor,
                Controlador = "Planes",
                Accion = nameof(Index),
                Filtros = filtros
            }
        });
    }

    [HttpGet]
    public IActionResult Crear(string? codigoEquipo) =>
        View(new CrearPlanViewModel { CodigoEquipo = codigoEquipo ?? string.Empty });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearPlanViewModel modelo, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var (creado, error) = await _planService.CrearAsync(modelo, ct);
        if (error != null || creado == null)
        {
            ViewBag.Error = error ?? "No se pudo crear el plan.";
            return View(modelo);
        }

        TempData["Mensaje"] = $"Plan #{creado.IdPlanMantenimiento} creado para {creado.CodigoEquipo}. Próximo mantenimiento: {creado.ProximaFecha:yyyy-MM-dd}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id, CancellationToken ct = default)
    {
        var (plan, error) = await _planService.ObtenerPorIdAsync(id, ct);
        if (error != null || plan == null)
        {
            TempData["Error"] = error ?? "No se encontró el plan.";
            return RedirectToAction(nameof(Index));
        }

        return View(new EditarPlanViewModel
        {
            IdPlanMantenimiento = plan.IdPlanMantenimiento,
            CodigoEquipo = plan.CodigoEquipo,
            EquipoNombre = plan.EquipoNombre,
            Frecuencia = plan.Frecuencia,
            ProximaFecha = plan.ProximaFecha.Date,
            IdEstado = plan.IdEstado
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(EditarPlanViewModel modelo, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var (actualizado, error) = await _planService.ActualizarAsync(modelo, ct);
        if (error != null || actualizado == null)
        {
            ViewBag.Error = error ?? "No se pudo actualizar el plan.";
            return View(modelo);
        }

        TempData["Mensaje"] = $"Plan #{actualizado.IdPlanMantenimiento} actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct = default)
    {
        var (_, error) = await _planService.EliminarAsync(id, ct);
        if (error != null)
            TempData["Error"] = error;
        else
            TempData["Mensaje"] = $"Plan #{id} eliminado. Las órdenes ya generadas se conservan.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EjecutarReglas(string? volver, CancellationToken ct = default)
    {
        var (resultado, error) = await _planService.EjecutarReglasAutomaticasAsync(ct);
        if (error != null || resultado == null)
        {
            TempData["Error"] = error ?? "No se pudieron ejecutar las reglas automáticas.";
        }
        else if (resultado.OrdenesEscaladas == 0 && resultado.OrdenesPreventivasGeneradas == 0)
        {
            TempData["Mensaje"] = "Reglas ejecutadas: no había órdenes críticas por escalar ni planes vencidos.";
        }
        else
        {
            static string Lista(List<int> n) => n.Count == 0 ? "" : $" (#{string.Join(", #", n)})";
            TempData["Mensaje"] =
                $"Proceso completado: {resultado.OrdenesEscaladas} orden(es) crítica(s) escalada(s){Lista(resultado.NumerosOrdenesEscaladas)} " +
                $"y {resultado.OrdenesPreventivasGeneradas} orden(es) preventiva(s) generada(s){Lista(resultado.NumerosOrdenesGeneradas)}.";
        }

        return volver == "BandejaSupervisor"
            ? RedirectToAction("BandejaSupervisor", "Ordenes")
            : RedirectToAction(nameof(Index));
    }
}
