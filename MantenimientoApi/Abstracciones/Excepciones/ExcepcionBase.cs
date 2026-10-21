namespace Abstracciones.Excepciones;

/// <summary>
/// Clase base para las excepciones del dominio y negocio del sistema.
/// </summary>
public abstract class ExcepcionBase : Exception
{
    /// <summary>
    /// Código de estado HTTP asociado a la excepción.
    /// </summary>
    public int CodigoEstado { get; }

    /// <summary>
    /// Título descriptivo del error para el formato ProblemDetails.
    /// </summary>
    public string Titulo { get; }

    protected ExcepcionBase(string mensaje, int codigoEstado, string titulo) : base(mensaje)
    {
        CodigoEstado = codigoEstado;
        Titulo = titulo;
    }
}
