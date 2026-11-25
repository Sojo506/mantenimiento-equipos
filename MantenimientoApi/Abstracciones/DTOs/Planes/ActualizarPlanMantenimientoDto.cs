using System.ComponentModel.DataAnnotations;

namespace Abstracciones.DTOs.Planes;

public class ActualizarPlanMantenimientoDto
{
    [Range(1, 365, ErrorMessage = "La frecuencia debe estar entre 1 y 365 días.")]
    public int Frecuencia { get; set; }

    [Required(ErrorMessage = "La próxima fecha de mantenimiento es obligatoria.")]
    public DateTime? ProximaFecha { get; set; }

    [Range(1, 2, ErrorMessage = "El estado del plan debe ser 1 (Activo) o 2 (Inactivo).")]
    public int IdEstado { get; set; } = 1;
}
