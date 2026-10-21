using System.ComponentModel.DataAnnotations;

namespace Abstracciones.DTOs.Ordenes;

/// <summary>
/// DTO de entrada para cambiar el estado de una orden de trabajo mediante la máquina de estados.
/// </summary>
public class CambiarEstadoOrdenDto
{
    [Range(7, 12, ErrorMessage = "El nuevo estado debe estar entre 7 (Abierta) y 12 (Escalada).")]
    public int NuevoEstado { get; set; }

    [Required(ErrorMessage = "La identificación del usuario que realiza la acción es obligatoria.")]
    public string IdentificacionUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "El comentario o justificación del cambio de estado es obligatorio.")]
    [StringLength(500, MinimumLength = 3, ErrorMessage = "El comentario debe tener entre 3 y 500 caracteres.")]
    public string Comentario { get; set; } = string.Empty;

    public decimal? TiempoInvertidoFinal { get; set; }
    public bool? SinRepuestos { get; set; }
    public string? JustificacionSinRepuestos { get; set; }

    [Required(ErrorMessage = "El token de concurrencia (RowVersion) es obligatorio.")]
    public string RowVersion { get; set; } = string.Empty;
}
