using System.ComponentModel.DataAnnotations;

namespace Abstracciones.DTOs.Repuestos;

/// <summary>
/// DTO de entrada para actualizar un repuesto existente.
/// Incluye RowVersion para control de concurrencia optimista.
/// </summary>
public class ActualizarRepuestoDto
{
    [Required(ErrorMessage = "El nombre del repuesto es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Range(0, 100000, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    [Range(0, 10000, ErrorMessage = "El stock mínimo no puede ser negativo.")]
    public int StockMinimo { get; set; }

    [Range(0.01, 1000000.00, ErrorMessage = "El costo unitario debe ser mayor a 0.")]
    public decimal CostoUnitario { get; set; }

    public int IdEstado { get; set; }

    [Required(ErrorMessage = "El token de concurrencia (RowVersion) es requerido para actualizar.")]
    public string RowVersion { get; set; } = string.Empty;
}
