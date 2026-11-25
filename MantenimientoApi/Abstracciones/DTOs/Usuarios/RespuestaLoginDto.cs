namespace Abstracciones.DTOs.Usuarios;

/// <summary>
/// DTO de salida con el token JWT emitido tras un inicio de sesión exitoso.
/// </summary>
public class RespuestaLoginDto
{
    public string Mensaje { get; set; } = "Inicio de sesión exitoso.";
    public string Token { get; set; } = string.Empty;
    public string TipoToken { get; set; } = "Bearer";
    public DateTime Expiracion { get; set; }
    public UsuarioDto Usuario { get; set; } = new();
}
