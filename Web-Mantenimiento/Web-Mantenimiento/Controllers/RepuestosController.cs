using Microsoft.AspNetCore.Mvc;
using Web_Mantenimiento.Filtros;
using Web_Mantenimiento.Models.Comun;
using Web_Mantenimiento.Models.Repuestos;
using Web_Mantenimiento.Services;

namespace Web_Mantenimiento.Controllers;

[AutorizacionSesion("Administrador", "Supervisor", "Tecnico")]
public class RepuestosController : Controller
{
    private readonly ApiRepuestoService _repuestoService;

    public RepuestosController(ApiRepuestoService repuestoService)
    {
        _repuestoService = repuestoService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? busqueda,
        string? ordenarPor,
        bool soloBajoMinimo = false,
        int pagina = 1,
        int tamanoPagina = 10,
        CancellationToken ct = default)
    {
        var (bajoMinimo, _) = await _repuestoService.ObtenerBajoMinimoAsync(ct);
        var vm = new RepuestoListadoViewModel
        {
            Busqueda = busqueda,
            OrdenarPor = ordenarPor,
            SoloBajoMinimo = soloBajoMinimo,
            TotalBajoMinimo = bajoMinimo?.Count ?? 0
        };

        if (soloBajoMinimo)
        {
            vm.Repuestos = bajoMinimo ?? new List<RepuestoViewModel>();
            return View(vm);
        }

        var (resultado, error) = await _repuestoService.ObtenerPaginadoAsync(
            busqueda, ordenarPor, pagina, tamanoPagina, ct);

        if (error != null)
            ViewBag.Error = error;

        vm.Repuestos = resultado?.Items ?? new List<RepuestoViewModel>();
        vm.Paginacion = new PaginacionViewModel
        {
            Meta = resultado?.Paginacion ?? new PaginacionMeta(),
            Busqueda = busqueda,
            OrdenarPor = ordenarPor,
            Controlador = "Repuestos",
            Accion = nameof(Index)
        };

        return View(vm);
    }

    [HttpGet]
    [AutorizacionSesion("Administrador", "Supervisor")]
    public IActionResult Crear() => View(new CrearRepuestoViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AutorizacionSesion("Administrador", "Supervisor")]
    public async Task<IActionResult> Crear(CrearRepuestoViewModel modelo, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var (_, error) = await _repuestoService.CrearAsync(modelo, ct);
        if (error != null)
        {
            ViewBag.Error = error;
            return View(modelo);
        }

        TempData["Mensaje"] = $"Repuesto {modelo.CodigoRepuesto} registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [AutorizacionSesion("Administrador", "Supervisor")]
    public async Task<IActionResult> Editar(string codigoRepuesto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(codigoRepuesto))
            return RedirectToAction(nameof(Index));

        var (repuesto, error) = await _repuestoService.ObtenerPorCodigoAsync(codigoRepuesto, ct);
        if (error != null || repuesto == null)
        {
            TempData["Error"] = error ?? "No se encontró el repuesto.";
            return RedirectToAction(nameof(Index));
        }

        var vm = new ActualizarRepuestoViewModel
        {
            CodigoRepuesto = repuesto.CodigoRepuesto,
            Nombre = repuesto.Nombre,
            Stock = repuesto.Stock,
            StockMinimo = repuesto.StockMinimo,
            CostoUnitario = repuesto.CostoUnitario,
            IdEstado = repuesto.IdEstado,
            RowVersion = repuesto.RowVersion
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AutorizacionSesion("Administrador", "Supervisor")]
    public async Task<IActionResult> Editar(ActualizarRepuestoViewModel modelo, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var (_, error) = await _repuestoService.ActualizarAsync(modelo.CodigoRepuesto, modelo, ct);
        if (error != null)
        {
            ViewBag.Error = error;
            return View(modelo);
        }

        TempData["Mensaje"] = $"Repuesto {modelo.CodigoRepuesto} actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AutorizacionSesion("Administrador", "Supervisor")]
    public async Task<IActionResult> Eliminar(string codigoRepuesto, CancellationToken ct = default)
    {
        var (_, error) = await _repuestoService.EliminarAsync(codigoRepuesto, ct);
        if (error != null)
            TempData["Error"] = error;
        else
            TempData["Mensaje"] = $"Repuesto {codigoRepuesto} eliminado correctamente.";

        return RedirectToAction(nameof(Index));
    }
}
