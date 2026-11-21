namespace Abstracciones.DTOs.Tablero;

public class RepuestosBajoMinimoDto
{
    public string CodigoRepuesto { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
    public int Deficit { get; set; }
    public decimal CostoUnitario { get; set; }
}