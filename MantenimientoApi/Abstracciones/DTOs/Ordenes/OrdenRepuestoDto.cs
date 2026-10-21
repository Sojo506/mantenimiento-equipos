namespace Abstracciones.DTOs.Ordenes;

/// <summary>
/// DTO para transferir información del repuesto consumido en una orden.
/// </summary>
public class OrdenRepuestoDto
{
    public string CodigoRepuesto { get; set; } = string.Empty;
    public string NombreRepuesto { get; set; } = string.Empty;
    public int CantidadUsada { get; set; }
    public decimal CostoAplicado { get; set; }
    public decimal Subtotal => CantidadUsada * CostoAplicado;
}
