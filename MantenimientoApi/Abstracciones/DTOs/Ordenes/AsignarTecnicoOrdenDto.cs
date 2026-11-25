using System.ComponentModel.DataAnnotations;

namespace Abstracciones.DTOs.Ordenes;

public class AsignarTecnicoOrdenDto
{
    [Required(ErrorMessage = "La identificación del técnico es obligatoria.")]
    public string IdentificacionTecnico { get; set; } = string.Empty;

    [Required(ErrorMessage = "La identificación del usuario que realiza la asignación es obligatoria.")]
    public string IdentificacionUsuario { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "El comentario no puede superar 500 caracteres.")]
    public string? Comentario { get; set; }

    [Required(ErrorMessage = "El token de concurrencia (RowVersion) es obligatorio.")]
    public string RowVersion { get; set; } = string.Empty;
}
