using System.Text.RegularExpressions;

namespace API.Restricciones;

/// <summary>
/// Restricción de ruta personalizada que valida que un parámetro de segmento URL
/// cumpla con el formato estándar de código de activo o repuesto (ej. EQ-001, REP-001).
/// </summary>
public class CodigoRutaConstraint : IRouteConstraint
{
    private static readonly Regex RegexCodigo = new(@"^[A-Za-z0-9]+(-[A-Za-z0-9]+)*$", RegexOptions.Compiled);

    public bool Match(
        HttpContext? httpContext, 
        IRouter? route, 
        string routeKey, 
        RouteValueDictionary values, 
        RouteDirection routeDirection)
    {
        if (values.TryGetValue(routeKey, out var valor) && valor != null)
        {
            string valorString = Convert.ToString(valor) ?? string.Empty;
            return RegexCodigo.IsMatch(valorString);
        }

        return false;
    }
}
