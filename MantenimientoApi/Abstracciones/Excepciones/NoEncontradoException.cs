namespace Abstracciones.Excepciones;

/// <summary>
/// Excepción lanzada cuando un recurso solicitado no existe en el sistema (HTTP 404).
/// </summary>
public class NoEncontradoException : ExcepcionBase
{
    public NoEncontradoException(string mensaje) 
        : base(mensaje, 404, "Recurso no encontrado")
    {
    }

    public NoEncontradoException(string entidad, object clave) 
        : base($"No se encontró la entidad '{entidad}' con identificador '{clave}'.", 404, "Recurso no encontrado")
    {
    }
}
