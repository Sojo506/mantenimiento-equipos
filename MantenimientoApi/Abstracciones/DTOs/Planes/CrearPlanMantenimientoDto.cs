using System.ComponentModel.DataAnnotations;

namespace Abstracciones.DTOs.Planes;

public class CrearPlanMantenimientoDto
{
    [Required(ErrorMessage = "El código de equipo es obligatorio.")]
    [StringLength(50, ErrorMessage = "El código de equipo no puede superar 50 caracteres.")]
    public string CodigoEquipo { get; set; } = string.Empty;

    [Range(1, 365, ErrorMessage = "La frecuencia debe estar entre 1 y 365 días.")]
    public int Frecuencia { get; set; }

    [Required(ErrorMessage = "La próxima fecha de mantenimiento es obligatoria.")]
    public DateTime? ProximaFecha { get; set; }
}
