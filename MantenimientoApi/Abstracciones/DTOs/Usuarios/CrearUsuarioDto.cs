using System.ComponentModel.DataAnnotations;

namespace Abstracciones.DTOs.Usuarios;

public class CrearUsuarioDto
{
    [Required(ErrorMessage = "La identificación es obligatoria.")]
    public string Identificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El primer apellido es obligatorio.")]
    public string PrimerApellido { get; set; } = string.Empty;

    public string SegundoApellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de usuario es obligatorio.")]
    public int IdTipoUsuario { get; set; }

    public int? IdEspecialidad { get; set; }

    [Required(ErrorMessage = "El estado es obligatorio.")]
    public int IdEstado { get; set; }
}