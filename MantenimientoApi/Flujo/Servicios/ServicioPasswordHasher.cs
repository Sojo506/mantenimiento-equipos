using Abstracciones.Interfaces;
using Abstracciones.Seguridad;

namespace Flujo.Servicios;

/// <summary>
/// Implementación del servicio de hashing seguro de contraseñas utilizando PBKDF2 y HMAC-SHA256.
/// </summary>
public class ServicioPasswordHasher : IServicioPasswordHasher
{
    public string HashearPassword(string password)
    {
        return HashHelper.Hashear(password);
    }

    public bool VerificarPassword(string password, string hash)
    {
        return HashHelper.Verificar(password, hash);
    }
}
