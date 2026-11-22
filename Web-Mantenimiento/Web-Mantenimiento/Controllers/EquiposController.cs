using Microsoft.AspNetCore.Mvc;
using Web_Mantenimiento.Filtros;
using Web_Mantenimiento.Models.Comun;
using Web_Mantenimiento.Models.Equipos;
using Web_Mantenimiento.Services;

namespace Web_Mantenimiento.Controllers;

[AutorizacionSesion]
public class EquiposController : Controller
{
    private readonly ApiEquipoService _equipoService;

    public EquiposController(ApiEquipoService equipoService)
    {
        _equipoService = equipoService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? busqueda,
        string? ordenarPor,
        int pagina = 1,
        int tamanoPagina = 10,
        CancellationToken ct = default)
    {
        var (resultado, error) = await _equipoService.ObtenerPaginadoAsync(
            busqueda, ordenarPor, pagina, tamanoPagina, ct);

        if (error != null)
            ViewBag.Error = error;

        var vm = new EquipoListadoViewModel
        {
            Equipos = resultado?.Items ?? new List<EquipoViewModel>(),
            Busqueda = busqueda,
            OrdenarPor = ordenarPor,
            Paginacion = new PaginacionViewModel
            {
                Meta = resultado?.Paginacion ?? new PaginacionMeta(),
                Busqueda = busqueda,
                OrdenarPor = ordenarPor,
                Controlador = "Equipos",
                Accion = nameof(Index)
            }
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(string codigoEquipo, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(codigoEquipo))
            return RedirectToAction(nameof(Index));

        var (equipo, error) = await _equipoService.ObtenerPorCodigoAsync(codigoEquipo, ct);
        if (error != null || equipo == null)
        {
            TempData["Error"] = error ?? "No se encontró el equipo solicitado.";
            return RedirectToAction(nameof(Index));
        }

        return View(equipo);
    }

    [HttpGet]
    [AutorizacionSesion("Administrador", "Supervisor")]
    public IActionResult Crear()
    {
        return View(new CrearEquipoViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AutorizacionSesion("Administrador", "Supervisor")]
    public async Task<IActionResult> Crear(CrearEquipoViewModel modelo, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var (_, error) = await _equipoService.CrearAsync(modelo, ct);
        if (error != null)
        {
            ViewBag.Error = error;
            return View(modelo);
        }

        TempData["Mensaje"] = $"Equipo {modelo.CodigoEquipo} registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [AutorizacionSesion("Administrador", "Supervisor")]
    public async Task<IActionResult> Editar(string codigoEquipo, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(codigoEquipo))
            return RedirectToAction(nameof(Index));

        var (equipo, error) = await _equipoService.ObtenerPorCodigoAsync(codigoEquipo, ct);
        if (error != null || equipo == null)
        {
            TempData["Error"] = error ?? "No se encontró el equipo.";
            return RedirectToAction(nameof(Index));
        }

        var vm = new ActualizarEquipoViewModel
        {
            CodigoEquipo = equipo.CodigoEquipo,
            Nombre = equipo.Nombre,
            IdTipoEquipo = equipo.IdTipoEquipo,
            IdMarca = equipo.IdMarca,
            IdModelo = equipo.IdModelo,
            IdUbicacion = equipo.IdUbicacion,
            IdEstado = equipo.IdEstado
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AutorizacionSesion("Administrador", "Supervisor")]
    public async Task<IActionResult> Editar(ActualizarEquipoViewModel modelo, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var (_, error) = await _equipoService.ActualizarAsync(modelo.CodigoEquipo, modelo, ct);
        if (error != null)
        {
            ViewBag.Error = error;
            return View(modelo);
        }

        TempData["Mensaje"] = $"Equipo {modelo.CodigoEquipo} actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AutorizacionSesion("Administrador", "Supervisor")]
    public async Task<IActionResult> Eliminar(string codigoEquipo, CancellationToken ct = default)
    {
        var (_, error) = await _equipoService.EliminarAsync(codigoEquipo, ct);
        if (error != null)
            TempData["Error"] = error;
        else
            TempData["Mensaje"] = $"Equipo {codigoEquipo} eliminado correctamente.";

        return RedirectToAction(nameof(Index));
    }
}
