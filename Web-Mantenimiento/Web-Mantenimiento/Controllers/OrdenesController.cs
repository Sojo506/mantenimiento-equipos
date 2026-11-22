using Microsoft.AspNetCore.Mvc;
using Web_Mantenimiento.Filtros;
using Web_Mantenimiento.Models.Comun;
using Web_Mantenimiento.Models.Ordenes;
using Web_Mantenimiento.Services;

namespace Web_Mantenimiento.Controllers;

[AutorizacionSesion]
public class OrdenesController : Controller
{
    private readonly ApiOrdenService _ordenService;

    public OrdenesController(ApiOrdenService ordenService)
    {
        _ordenService = ordenService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? busqueda, string? ordenarPor, int? idEstado, string? prioridad,
        int pagina = 1, CancellationToken ct = default)
    {
        var rol = HttpContext.Session.GetString("Rol");
        var idUsuario = HttpContext.Session.GetString("Identificacion");
        var filtroSolicitante = rol == "Solicitante" ? idUsuario : null;

        var titulo = rol == "Solicitante" ? "Mis Solicitudes de Mantenimiento" : "Órdenes de Trabajo";
        return await RenderListadoAsync(
            titulo, "Listado de órdenes registradas", nameof(Index),
            busqueda, ordenarPor, idEstado, prioridad, null, filtroSolicitante, pagina, ct);
    }

    [HttpGet]
    [AutorizacionSesion("Tecnico", "Administrador")]
    public Task<IActionResult> BandejaTecnico(
        string? busqueda, string? ordenarPor, int? idEstado, string? prioridad,
        int pagina = 1, CancellationToken ct = default)
    {
        var idUsuario = HttpContext.Session.GetString("Identificacion");
        return RenderListadoAsync(
            "Bandeja del Técnico", "Órdenes asignadas a tu usuario", nameof(BandejaTecnico),
            busqueda, ordenarPor, idEstado, prioridad, idUsuario, null, pagina, ct);
    }

    [HttpGet]
    [AutorizacionSesion("Supervisor", "Administrador")]
    public Task<IActionResult> BandejaSupervisor(
        string? busqueda, string? ordenarPor, int? idEstado, string? prioridad,
        int pagina = 1, CancellationToken ct = default)
    {
        return RenderListadoAsync(
            "Bandeja del Supervisor", "Supervisión, escalamientos y seguimiento operativo", nameof(BandejaSupervisor),
            busqueda, ordenarPor ?? "limite", idEstado, prioridad, null, null, pagina, ct);
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int numero, CancellationToken ct = default)
    {
        var (orden, error) = await _ordenService.ObtenerPorNumeroAsync(numero, ct);
        if (error != null || orden == null)
        {
            TempData["Error"] = error ?? "No se encontró la orden solicitada.";
            return RedirectToAction(nameof(Index));
        }

        return View(orden);
    }

    [HttpGet]
    public IActionResult Crear() => View(new CrearOrdenViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearOrdenViewModel modelo, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var idUsuario = HttpContext.Session.GetString("Identificacion") ?? string.Empty;
        var (creada, error) = await _ordenService.CrearAsync(modelo, idUsuario, ct);
        if (error != null || creada == null)
        {
            ViewBag.Error = error ?? "No se pudo crear la orden.";
            return View(modelo);
        }

        TempData["Mensaje"] = $"Orden #{creada.Numero} creada correctamente.";
        return RedirectToAction(nameof(Detalle), new { numero = creada.Numero });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AutorizacionSesion("Administrador", "Supervisor", "Tecnico")]
    public async Task<IActionResult> CambiarEstado(CambiarEstadoOrdenViewModel modelo, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Revise los datos requeridos para cambiar el estado.";
            return RedirectToAction(nameof(Detalle), new { numero = modelo.Numero });
        }

        var idUsuario = HttpContext.Session.GetString("Identificacion") ?? string.Empty;
        var (_, error) = await _ordenService.CambiarEstadoAsync(modelo, idUsuario, ct);
        if (error != null)
            TempData["Error"] = error;
        else
            TempData["Mensaje"] = modelo.NuevoEstado == 11
                ? $"Orden #{modelo.Numero} cerrada correctamente."
                : $"Estado de la orden #{modelo.Numero} actualizado correctamente.";

        return RedirectToAction(nameof(Detalle), new { numero = modelo.Numero });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AutorizacionSesion("Administrador", "Supervisor", "Tecnico")]
    public async Task<IActionResult> AsignarRepuesto(AsignarRepuestoOrdenViewModel modelo, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Debe indicar un repuesto y una cantidad mayor a 0.";
            return RedirectToAction(nameof(Detalle), new { numero = modelo.Numero });
        }

        var (_, error) = await _ordenService.AsignarRepuestoAsync(modelo, ct);
        if (error != null)
            TempData["Error"] = error;
        else
            TempData["Mensaje"] = $"Repuesto {modelo.CodigoRepuesto} asignado a la orden #{modelo.Numero}.";

        return RedirectToAction(nameof(Detalle), new { numero = modelo.Numero });
    }

    [HttpGet]
    public async Task<IActionResult> Bitacora(int numero, CancellationToken ct = default)
    {
        var (historial, error) = await _ordenService.ObtenerHistorialAsync(numero, ct);
        if (error != null)
            ViewBag.Error = error;

        ViewData["NumeroOrden"] = numero;
        return View(historial ?? new List<BitacoraOrdenViewModel>());
    }

    private async Task<IActionResult> RenderListadoAsync(
        string titulo, string subtitulo, string accion,
        string? busqueda, string? ordenarPor, int? idEstado, string? prioridad,
        string? tecnico, string? solicitante, int pagina, CancellationToken ct)
    {
        var (resultado, error) = await _ordenService.ObtenerPaginadoAsync(
            busqueda, ordenarPor, idEstado, prioridad, tecnico, solicitante, pagina, 10, ct);

        if (error != null)
            ViewBag.Error = error;

        var vm = new OrdenListadoViewModel
        {
            Titulo = titulo,
            Subtitulo = subtitulo,
            AccionActual = accion,
            Ordenes = resultado?.Items ?? new List<OrdenViewModel>(),
            Busqueda = busqueda,
            OrdenarPor = ordenarPor,
            IdEstado = idEstado,
            Prioridad = prioridad,
            Paginacion = new PaginacionViewModel
            {
                Meta = resultado?.Paginacion ?? new PaginacionMeta(),
                Busqueda = busqueda,
                OrdenarPor = ordenarPor,
                Controlador = "Ordenes",
                Accion = accion
            }
        };

        return View("Index", vm);
    }
}
