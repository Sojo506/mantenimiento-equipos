namespace Abstracciones.DTOs.Usuarios;

/// <summary>
/// DTO de salida con el token JWT emitido tras un inicio de sesión exitoso.
/// </summary>
public class RespuestaLoginDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiracion { get; set; }
    public UsuarioDto Usuario { get; set; } = new();
}
