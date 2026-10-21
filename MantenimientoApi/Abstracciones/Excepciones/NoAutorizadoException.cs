namespace Abstracciones.Excepciones;

/// <summary>
/// Excepción lanzada cuando un usuario no tiene permisos suficientes para ejecutar una acción (HTTP 403).
/// </summary>
public class NoAutorizadoException : ExcepcionBase
{
    public NoAutorizadoException(string mensaje) 
        : base(mensaje, 403, "Acceso no autorizado")
    {
    }
}
