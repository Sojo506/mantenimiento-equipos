namespace Abstracciones.DTOs.Repuestos;

/// <summary>
/// DTO de salida para transferir información de un repuesto de inventario.
/// </summary>
public class RepuestoDto
{
    public string CodigoRepuesto { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
    public decimal CostoUnitario { get; set; }
    public int IdEstado { get; set; }
    public string EstadoNombre { get; set; } = string.Empty;
    public bool BajoStockMinimo => Stock <= StockMinimo;
    public string RowVersion { get; set; } = string.Empty;
}
