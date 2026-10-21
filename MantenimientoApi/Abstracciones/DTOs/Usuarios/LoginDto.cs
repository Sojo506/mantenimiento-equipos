using System.ComponentModel.DataAnnotations;

namespace Abstracciones.DTOs.Usuarios;

/// <summary>
/// DTO de entrada para autenticación en el sistema.
/// </summary>
public class LoginDto
{
    [Required(ErrorMessage = "El correo electrónico es requerido.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida.")]
    public string Password { get; set; } = string.Empty;
}
