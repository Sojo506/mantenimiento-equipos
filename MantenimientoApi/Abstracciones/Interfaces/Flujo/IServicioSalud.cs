namespace Abstracciones.Interfaces.Flujo;

public interface IServicioSalud
{
    Task<bool> VerificarConexionAsync(
        CancellationToken cancellationToken = default);
}