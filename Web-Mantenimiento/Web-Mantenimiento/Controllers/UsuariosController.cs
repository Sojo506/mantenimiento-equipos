
using Microsoft.AspNetCore.Mvc;
using Web_Mantenimiento.Filtros;
using Web_Mantenimiento.Models.Usuarios;
using Web_Mantenimiento.Services;

namespace Web_Mantenimiento.Controllers;

[AutorizacionSesion("Administrador")]
public class UsuariosController : Controller
{
    private readonly ApiUsuarioService _usuarioService;

    public UsuariosController(ApiUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var (usuarios, error) =
            await _usuarioService.ObtenerTodosAsync(cancellationToken);

        if (usuarios == null)
        {
            ViewBag.Error = error;
            return View(new List<UsuarioViewModel>());
        }

        return View(usuarios);
    }

    [HttpGet]
    public async Task<IActionResult> Detalles(
        string identificacion,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(identificacion))
            return RedirectToAction(nameof(Index));

        var (usuario, error) =
            await _usuarioService.ObtenerPorIdentificacionAsync(
                identificacion, cancellationToken);

        if (usuario == null)
        {
            TempData["Error"] = error ?? "Usuario no encontrado.";
            return RedirectToAction(nameof(Index));
        }

        return View(usuario);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View(new CrearUsuarioViewModel
        {
            IdEstado = 1
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        CrearUsuarioViewModel modelo,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var error = await _usuarioService.CrearAsync(
            modelo, cancellationToken);

        if (error != null)
        {
            ModelState.AddModelError(string.Empty, error);
            return View(modelo);
        }

        TempData["Mensaje"] = "Usuario creado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(
        string identificacion,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(identificacion))
            return RedirectToAction(nameof(Index));

        var (usuario, error) =
            await _usuarioService.ObtenerPorIdentificacionAsync(
                identificacion, cancellationToken);

        if (usuario == null)
        {
            TempData["Error"] = error ?? "Usuario no encontrado.";
            return RedirectToAction(nameof(Index));
        }

        var modelo = new ActualizarUsuarioViewModel
        {
            Identificacion = usuario.Identificacion,
            Nombre = usuario.Nombre,
            PrimerApellido = usuario.PrimerApellido,
            SegundoApellido = usuario.SegundoApellido,
            Correo = usuario.Correo,
            IdTipoUsuario = usuario.IdTipoUsuario,
            IdEspecialidad = usuario.IdEspecialidad,
            IdEstado = usuario.IdEstado,
            Disponibilidad = usuario.Disponibilidad
        };

        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        string identificacion,
        ActualizarUsuarioViewModel modelo,
        CancellationToken cancellationToken)
    {
        if (identificacion != modelo.Identificacion)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(modelo);

        var error = await _usuarioService.ActualizarAsync(
            identificacion, modelo, cancellationToken);

        if (error != null)
        {
            ModelState.AddModelError(string.Empty, error);
            return View(modelo);
        }

        TempData["Mensaje"] = "Usuario actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(
        string identificacion,
        CancellationToken cancellationToken)
    {
        var error = await _usuarioService.EliminarAsync(
            identificacion, cancellationToken);

        if (error != null)
            TempData["Error"] = error;
        else
            TempData["Mensaje"] = "Usuario eliminado correctamente.";

        return RedirectToAction(nameof(Index));
    }
}
