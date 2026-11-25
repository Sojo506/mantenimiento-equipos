using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web_Mantenimiento.Filtros;
using Web_Mantenimiento.Models.Comun;
using Web_Mantenimiento.Models.Ordenes;
using Web_Mantenimiento.Services;

namespace Web_Mantenimiento.Controllers;

[AutorizacionSesion]
public class OrdenesController : Controller
{
    private readonly ApiOrdenService _ordenService;
    private readonly ApiUsuarioService _usuarioService;

    public OrdenesController(ApiOrdenService ordenService, ApiUsuarioService usuarioService)
    {
        _ordenService = ordenService;
        _usuarioService = usuarioService;
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
        var vm = await ConstruirListadoAsync(
            titulo, "Listado de órdenes registradas", nameof(Index),
            busqueda, ordenarPor, idEstado, prioridad, null, filtroSolicitante, false, false, pagina, ct);

        return View("Index", vm);
    }

    [HttpGet]
    [AutorizacionSesion("Tecnico", "Administrador")]
    public async Task<IActionResult> BandejaTecnico(
        string? busqueda, string? ordenarPor, int? idEstado, string? prioridad,
        bool vencidas = false, int pagina = 1, CancellationToken ct = default)
    {
        var idUsuario = HttpContext.Session.GetString("Identificacion");

        var vm = await ConstruirListadoAsync(
            "Bandeja del Técnico", "Órdenes asignadas a tu usuario: atiéndelas, registra repuestos y ciérralas",
            nameof(BandejaTecnico), busqueda, ordenarPor ?? "limite", idEstado, prioridad,
            idUsuario, null, activas: idEstado == null, vencidas, pagina, ct);

        var conteos = await Task.WhenAll(
            _ordenService.ContarAsync(8, idUsuario, ct: ct),
            _ordenService.ContarAsync(9, idUsuario, ct: ct),
            _ordenService.ContarAsync(10, idUsuario, ct: ct),
            _ordenService.ContarAsync(12, idUsuario, ct: ct),
            _ordenService.ContarAsync(null, idUsuario, vencidas: true, ct: ct));

        vm.Resumen = new List<BandejaResumenItem>
        {
            new() { Etiqueta = "Por atender", Cantidad = conteos[0], IdEstado = 8, Clase = "text-bg-primary" },
            new() { Etiqueta = "En proceso", Cantidad = conteos[1], IdEstado = 9, Clase = "text-bg-info" },
            new() { Etiqueta = "Esperando repuesto", Cantidad = conteos[2], IdEstado = 10, Clase = "text-bg-warning" },
            new() { Etiqueta = "Escaladas", Cantidad = conteos[3], IdEstado = 12, Clase = "text-bg-danger" },
            new() { Etiqueta = "Vencidas", Cantidad = conteos[4], Vencidas = true, Clase = "text-bg-dark" }
        };

        return View(vm);
    }

    [HttpGet]
    [AutorizacionSesion("Supervisor", "Administrador")]
    public async Task<IActionResult> BandejaSupervisor(
        string? busqueda, string? ordenarPor, int? idEstado, string? prioridad,
        bool vencidas = false, int pagina = 1, CancellationToken ct = default)
    {
        var vm = await ConstruirListadoAsync(
            "Bandeja del Supervisor", "Asignación de técnicos, escalamientos y seguimiento de plazos",
            nameof(BandejaSupervisor), busqueda, ordenarPor ?? "limite", idEstado, prioridad,
            null, null, activas: idEstado == null, vencidas, pagina, ct);

        var conteos = await Task.WhenAll(
            _ordenService.ContarAsync(7, ct: ct),
            _ordenService.ContarAsync(12, ct: ct),
            _ordenService.ContarAsync(null, vencidas: true, ct: ct),
            _ordenService.ContarAsync(9, ct: ct),
            _ordenService.ContarAsync(10, ct: ct));

        vm.Resumen = new List<BandejaResumenItem>
        {
            new() { Etiqueta = "Sin asignar", Cantidad = conteos[0], IdEstado = 7, Clase = "text-bg-warning" },
            new() { Etiqueta = "Escaladas (RN-01)", Cantidad = conteos[1], IdEstado = 12, Clase = "text-bg-danger" },
            new() { Etiqueta = "Vencidas", Cantidad = conteos[2], Vencidas = true, Clase = "text-bg-dark" },
            new() { Etiqueta = "En proceso", Cantidad = conteos[3], IdEstado = 9, Clase = "text-bg-info" },
            new() { Etiqueta = "Esperando repuesto", Cantidad = conteos[4], IdEstado = 10, Clase = "text-bg-secondary" }
        };

        vm.Tecnicos = await ObtenerTecnicosAsync(ct);
        return View(vm);
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

        var rol = HttpContext.Session.GetString("Rol");
        if (rol is "Supervisor" or "Administrador" && !orden.EstaCerrada)
            ViewBag.Tecnicos = await ObtenerTecnicosAsync(ct);

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
        var resultado = await _ordenService.CambiarEstadoAsync(modelo, idUsuario, ct);

        string exito = modelo.NuevoEstado == 11
            ? $"Orden #{modelo.Numero} cerrada correctamente."
            : $"Estado de la orden #{modelo.Numero} actualizado a '{resultado.Datos?.EstadoNombre}'.";

        RegistrarResultado(resultado, modelo.Numero, exito);
        return RedirectToAction(nameof(Detalle), new { numero = modelo.Numero });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AutorizacionSesion("Administrador", "Tecnico")]
    public async Task<IActionResult> Atender(AtenderOrdenViewModel modelo, CancellationToken ct = default)
    {
        if (!ModelState.IsValid || modelo.NuevoEstado is not (9 or 10))
        {
            TempData["Error"] = "Acción no válida para la orden.";
            return RedirectToAction(nameof(BandejaTecnico));
        }

        var comentario = string.IsNullOrWhiteSpace(modelo.Comentario)
            ? modelo.NuevoEstado == 9 ? "El técnico inicia o reanuda la atención de la orden." : "Trabajo en pausa a la espera de repuestos."
            : modelo.Comentario.Trim();

        var cambio = new CambiarEstadoOrdenViewModel
        {
            Numero = modelo.Numero,
            NuevoEstado = modelo.NuevoEstado,
            Comentario = comentario,
            RowVersion = modelo.RowVersion
        };

        var idUsuario = HttpContext.Session.GetString("Identificacion") ?? string.Empty;
        var resultado = await _ordenService.CambiarEstadoAsync(cambio, idUsuario, ct);

        RegistrarResultado(resultado, modelo.Numero,
            modelo.NuevoEstado == 9 ? $"Orden #{modelo.Numero} en proceso." : $"Orden #{modelo.Numero} en espera de repuesto.");

        return RedirectToAction(nameof(BandejaTecnico));
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

        var resultado = await _ordenService.AsignarRepuestoAsync(modelo, ct);
        RegistrarResultado(resultado, modelo.Numero, $"Repuesto {modelo.CodigoRepuesto} (x{modelo.Cantidad}) asignado a la orden #{modelo.Numero}.");

        if (resultado.EsConflicto)
        {
            TempData["RepuestoPendiente"] = modelo.CodigoRepuesto;
            TempData["CantidadPendiente"] = modelo.Cantidad;
        }

        return RedirectToAction(nameof(Detalle), new { numero = modelo.Numero });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AutorizacionSesion("Administrador", "Supervisor")]
    public async Task<IActionResult> AsignarTecnico(AsignarTecnicoOrdenViewModel modelo, CancellationToken ct = default)
    {
        bool volverABandeja = modelo.Volver == nameof(BandejaSupervisor);

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Debe seleccionar un técnico.";
        }
        else
        {
            var idUsuario = HttpContext.Session.GetString("Identificacion") ?? string.Empty;
            var resultado = await _ordenService.AsignarTecnicoAsync(modelo, idUsuario, ct);
            RegistrarResultado(resultado, modelo.Numero,
                $"Orden #{modelo.Numero} asignada a {resultado.Datos?.TecnicoNombre ?? modelo.IdentificacionTecnico}.");
        }

        return volverABandeja
            ? RedirectToAction(nameof(BandejaSupervisor))
            : RedirectToAction(nameof(Detalle), new { numero = modelo.Numero });
    }

    [HttpGet]
    public async Task<IActionResult> Bitacora(int numero, CancellationToken ct = default)
    {
        var (historial, error) = await _ordenService.ObtenerHistorialAsync(numero, ct);
        if (error != null)
            ViewBag.Error = error;

        var (orden, _) = await _ordenService.ObtenerPorNumeroAsync(numero, ct);
        ViewBag.Orden = orden;
        ViewData["NumeroOrden"] = numero;

        return View((historial ?? new List<BitacoraOrdenViewModel>())
            .OrderBy(b => b.Fecha).ThenBy(b => b.IdBitacora).ToList());
    }

    private void RegistrarResultado<T>(RespuestaApi<T> resultado, int numero, string mensajeExito)
    {
        if (resultado.EsConflicto)
        {
            TempData["Advertencia"] =
                $"No se aplicó el cambio en la orden #{numero} (409 Conflict): {resultado.Error} " +
                "Se cargaron los datos más recientes; revíselos y repita la acción si aún corresponde.";
        }
        else if (!resultado.Exito)
        {
            TempData["Error"] = resultado.Error;
        }
        else
        {
            TempData["Mensaje"] = mensajeExito;
        }
    }

    private async Task<List<SelectListItem>> ObtenerTecnicosAsync(CancellationToken ct)
    {
        var (usuarios, _) = await _usuarioService.ObtenerTodosAsync(ct);
        return (usuarios ?? new())
            .Where(u => u.RolNombre == "Tecnico" && u.IdEstado == 1)
            .OrderBy(u => u.NombreCompleto)
            .Select(u => new SelectListItem(
                $"{u.NombreCompleto}{(string.IsNullOrWhiteSpace(u.EspecialidadNombre) ? "" : $" — {u.EspecialidadNombre}")}{(u.Disponibilidad ? "" : " (no disponible)")}",
                u.Identificacion))
            .ToList();
    }

    private async Task<OrdenListadoViewModel> ConstruirListadoAsync(
        string titulo, string subtitulo, string accion,
        string? busqueda, string? ordenarPor, int? idEstado, string? prioridad,
        string? tecnico, string? solicitante, bool activas, bool vencidas, int pagina, CancellationToken ct)
    {
        var (resultado, error) = await _ordenService.ObtenerPaginadoAsync(
            busqueda, ordenarPor, idEstado, prioridad, tecnico, solicitante, pagina, 10, ct, activas, vencidas);

        if (error != null)
            ViewBag.Error = error;

        return new OrdenListadoViewModel
        {
            Titulo = titulo,
            Subtitulo = subtitulo,
            AccionActual = accion,
            Ordenes = resultado?.Items ?? new List<OrdenViewModel>(),
            Busqueda = busqueda,
            OrdenarPor = ordenarPor,
            IdEstado = idEstado,
            Prioridad = prioridad,
            Activas = activas,
            Vencidas = vencidas,
            Paginacion = new PaginacionViewModel
            {
                Meta = resultado?.Paginacion ?? new PaginacionMeta(),
                Busqueda = busqueda,
                OrdenarPor = ordenarPor,
                Controlador = "Ordenes",
                Accion = accion,
                Filtros = new Dictionary<string, string?>
                {
                    ["idEstado"] = idEstado?.ToString(),
                    ["prioridad"] = prioridad,
                    ["vencidas"] = vencidas ? "true" : null
                }
                .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                .ToDictionary(kv => kv.Key, kv => kv.Value!)
            }
        };
    }
}
