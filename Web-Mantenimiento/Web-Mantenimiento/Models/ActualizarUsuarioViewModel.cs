
using System.ComponentModel.DataAnnotations;

namespace Web_Mantenimiento.Models.Usuarios;

public class ActualizarUsuarioViewModel
{
    public string Identificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El primer apellido es obligatorio.")]
    public string PrimerApellido { get; set; } = string.Empty;

    public string SegundoApellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
    public string Correo { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un tipo de usuario.")]
    public int IdTipoUsuario { get; set; }

    public int? IdEspecialidad { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un estado.")]
    public int IdEstado { get; set; }

    public bool Disponibilidad { get; set; }
}
