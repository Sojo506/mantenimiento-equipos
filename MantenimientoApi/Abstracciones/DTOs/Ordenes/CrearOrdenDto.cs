using System.ComponentModel.DataAnnotations;

namespace Abstracciones.DTOs.Ordenes;

/// <summary>
/// DTO de entrada para crear una nueva orden de trabajo.
/// </summary>
public class CrearOrdenDto
{
    [Required(ErrorMessage = "El código de equipo es obligatorio.")]
    public string CodigoEquipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La identificación del solicitante es obligatoria.")]
    public string IdentificacionSolicitante { get; set; } = string.Empty;

    public string? IdentificacionTecnico { get; set; }

    [Range(1, 2, ErrorMessage = "El tipo de orden debe ser 1 (Preventiva) o 2 (Correctiva).")]
    public int IdTipoOrden { get; set; }

    [Required(ErrorMessage = "La prioridad es obligatoria.")]
    [RegularExpression("^(Baja|Media|Alta|Critica)$", ErrorMessage = "La prioridad debe ser: Baja, Media, Alta o Critica.")]
    public string Prioridad { get; set; } = "Media";

    [Required(ErrorMessage = "La descripción del trabajo o falla es obligatoria.")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "La descripción debe tener entre 5 y 500 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;
}
