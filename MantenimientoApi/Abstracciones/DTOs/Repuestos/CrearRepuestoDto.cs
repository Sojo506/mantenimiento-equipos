using System.ComponentModel.DataAnnotations;

namespace Abstracciones.DTOs.Repuestos;

/// <summary>
/// DTO de entrada para registrar un nuevo repuesto.
/// </summary>
public class CrearRepuestoDto
{
    [Required(ErrorMessage = "El código del repuesto es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El código debe tener entre 3 y 50 caracteres.")]
    [Abstracciones.Validaciones.CodigoFormato]
    public string CodigoRepuesto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del repuesto es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Range(0, 100000, ErrorMessage = "El stock inicial no puede ser negativo.")]
    public int Stock { get; set; }

    [Range(0, 10000, ErrorMessage = "El stock mínimo no puede ser negativo.")]
    public int StockMinimo { get; set; }

    [Range(0.01, 1000000.00, ErrorMessage = "El costo unitario debe ser mayor a 0.")]
    public decimal CostoUnitario { get; set; }

    public int IdEstado { get; set; } = 1;
}
