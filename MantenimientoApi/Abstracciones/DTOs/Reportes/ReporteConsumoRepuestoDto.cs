namespace Abstracciones.DTOs.Reportes;

/// <summary>
/// DTO con el resultado del procedimiento almacenado sp_ReporteConsumoRepuestosPorPeriodo.
/// </summary>
public class ReporteConsumoRepuestoDto
{
    public string CodigoRepuesto { get; set; } = string.Empty;
    public string NombreRepuesto { get; set; } = string.Empty;
    public int CantidadTotalConsumida { get; set; }
    public decimal CostoTotalAcumulado { get; set; }
    public int NumeroDeOrdenes { get; set; }
}
