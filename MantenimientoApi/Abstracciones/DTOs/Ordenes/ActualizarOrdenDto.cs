using System.ComponentModel.DataAnnotations;

namespace Abstracciones.DTOs.Ordenes;

/// <summary>
/// DTO de entrada para actualizar datos generales de una orden de trabajo.
/// </summary>
public class ActualizarOrdenDto
{
    public string? IdentificacionTecnico { get; set; }

    [Required(ErrorMessage = "La prioridad es obligatoria.")]
    [RegularExpression("^(Baja|Media|Alta|Critica)$", ErrorMessage = "La prioridad debe ser: Baja, Media, Alta o Critica.")]
    public string Prioridad { get; set; } = "Media";

    [Required(ErrorMessage = "La descripción es requerida.")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "La descripción debe tener entre 5 y 500 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    [Range(0, 1000, ErrorMessage = "El tiempo invertido en horas debe ser mayor o igual a 0.")]
    public decimal TiempoInvertido { get; set; }

    public bool SinRepuestos { get; set; }
    public string? JustificacionSinRepuestos { get; set; }

    [Required(ErrorMessage = "El token de concurrencia es obligatorio.")]
    public string RowVersion { get; set; } = string.Empty;
}
