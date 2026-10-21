namespace Abstracciones.Excepciones;

/// <summary>
/// Excepción lanzada cuando una operación viola una regla de negocio del sistema (HTTP 400).
/// </summary>
public class ReglaNegocioException : ExcepcionBase
{
    public ReglaNegocioException(string mensaje) 
        : base(mensaje, 400, "Violación de regla de negocio")
    {
    }
}
