namespace Abstracciones.Interfaces;

/// <summary>
/// Contrato para el servicio de generación y verificación de hashes de contraseñas.
/// </summary>
public interface IServicioPasswordHasher
{
    string HashearPassword(string password);
    bool VerificarPassword(string password, string hash);
}
