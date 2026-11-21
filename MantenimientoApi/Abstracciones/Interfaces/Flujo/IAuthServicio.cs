using Abstracciones.DTOs.Usuarios;
using Abstracciones.Entidades;

namespace Abstracciones.Interfaces.Flujo

/// <summary>
/// Contrato para la lógica de negocio relacionada con la autenticación de usuarios.
/// </summary>
{
    public interface IAuthServicio
    {
        /// <summary>
        /// Valida las credenciales del usuario y genera una respuesta con el token JWT.
        /// </summary>
        Task<RespuestaLoginDto?> LoginAsync(LoginDto loginDto);

        /// <summary>
        /// Genera un token JWT a partir de los datos del usuario.
        /// </summary>
        string GenerarToken(Usuario usuario);

    }
}
