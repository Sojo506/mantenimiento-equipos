using Microsoft.AspNetCore.Mvc;
using Web_Mantenimiento.Filtros;

namespace Web_Mantenimiento.Controllers;

[AutorizacionSesion]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
            return RedirectToAction("Login", "Account");

        return View();
    }
}