namespace Abstracciones.Excepciones;

/// <summary>
/// Excepción lanzada cuando la operación entra en conflicto con el estado actual del recurso
/// o ante colisiones de concurrencia optimista (HTTP 409).
/// </summary>
public class ConflictoException : ExcepcionBase
{
    public ConflictoException(string mensaje) 
        : base(mensaje, 409, "Conflicto con el estado actual del recurso")
    {
    }
}
