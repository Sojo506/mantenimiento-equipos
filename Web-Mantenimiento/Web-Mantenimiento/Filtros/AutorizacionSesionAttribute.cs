
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web_Mantenimiento.Filtros;

public class AutorizacionSesionAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _rolesPermitidos;

    public AutorizacionSesionAttribute(params string[] rolesPermitidos)
    {
        _rolesPermitidos = rolesPermitidos;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var sesion = context.HttpContext.Session;

        var token = sesion.GetString("JwtToken");
        var rol = sesion.GetString("Rol");

        if (string.IsNullOrWhiteSpace(token))
        {
            context.Result = new RedirectToActionResult(
                "Login", "Account", null);
            return;
        }

        if (_rolesPermitidos.Length > 0 &&
            !_rolesPermitidos.Contains(rol))
        {
            context.Result = new RedirectToActionResult(
                "Index", "Home", null);
        }
    }
}
