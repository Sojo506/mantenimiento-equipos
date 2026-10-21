using System.ComponentModel.DataAnnotations;

namespace Abstracciones.DTOs.Ordenes;

/// <summary>
/// DTO de entrada para registrar el uso de un repuesto en una orden de trabajo.
/// </summary>
public class AsignarRepuestoOrdenDto
{
    [Required(ErrorMessage = "El código del repuesto es obligatorio.")]
    public string CodigoRepuesto { get; set; } = string.Empty;

    [Range(1, 1000, ErrorMessage = "La cantidad usada debe ser mayor a 0.")]
    public int Cantidad { get; set; }
}
