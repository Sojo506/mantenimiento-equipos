
using Microsoft.AspNetCore.Mvc;
using Web_Mantenimiento.Models;
using Web_Mantenimiento.Services;

namespace Web_Mantenimiento.Controllers;

public class AccountController : Controller
{
    private readonly ApiAuthService _authService;

    public AccountController(ApiAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel modelo,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var (respuesta, error) = await _authService.LoginAsync(
            modelo.Correo,
            modelo.Password,
            cancellationToken);

        if (respuesta == null)
        {
            ModelState.AddModelError(string.Empty,
                error ?? "No fue posible iniciar sesión.");

            return View(modelo);
        }

        // Guardar el JWT y los datos del usuario en sesión.
        HttpContext.Session.SetString("JwtToken", respuesta.Token);
        HttpContext.Session.SetString(
            "Identificacion", respuesta.Usuario.Identificacion);
        HttpContext.Session.SetString(
            "Nombre", respuesta.Usuario.Nombre);
        HttpContext.Session.SetString(
            "Correo", respuesta.Usuario.Correo);
        HttpContext.Session.SetString(
            "Rol", respuesta.Usuario.Rol);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction(nameof(Login));
    }
}
