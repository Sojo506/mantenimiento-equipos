using System.Net;

namespace Web_Mantenimiento.Models.Comun;

public class RespuestaApi<T>
{
    public T? Datos { get; init; }
    public string? Error { get; init; }
    public HttpStatusCode? CodigoEstado { get; init; }

    public bool Exito => Error == null;
    public bool EsConflicto => CodigoEstado == HttpStatusCode.Conflict;
}
