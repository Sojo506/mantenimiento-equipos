namespace Web_Mantenimiento.Models.Reportes;

public class ReporteConsumoRepuestoViewModel
{
    public string CodigoRepuesto { get; set; } = string.Empty;
    public string NombreRepuesto { get; set; } = string.Empty;
    public int CantidadTotalConsumida { get; set; }
    public decimal CostoTotalAcumulado { get; set; }
    public int NumeroDeOrdenes { get; set; }
}

public class ReportesFiltroViewModel
{
    public DateTime FechaInicio { get; set; } = new DateTime(2026, 1, 1);
    public DateTime FechaFin { get; set; } = DateTime.Today.AddDays(1);
    public List<ReporteConsumoRepuestoViewModel> ConsumoRepuestos { get; set; } = new();
}
