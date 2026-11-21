
using System.ComponentModel.DataAnnotations;

namespace Web_Mantenimiento.Models.Usuarios;

public class CrearUsuarioViewModel
{
    [Required(ErrorMessage = "La identificación es obligatoria.")]
    public string Identificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El primer apellido es obligatorio.")]
    public string PrimerApellido { get; set; } = string.Empty;

    public string SegundoApellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un tipo de usuario.")]
    public int IdTipoUsuario { get; set; }

    public int? IdEspecialidad { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un estado.")]
    public int IdEstado { get; set; }
}
