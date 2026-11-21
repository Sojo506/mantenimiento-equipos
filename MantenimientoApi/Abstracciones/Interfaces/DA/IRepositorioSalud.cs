namespace Abstracciones.Interfaces.DA;

public interface IRepositorioSalud
{
    Task<bool> VerificarConexionAsync(
        CancellationToken cancellationToken = default);
}