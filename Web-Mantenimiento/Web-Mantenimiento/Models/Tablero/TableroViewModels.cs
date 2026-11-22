namespace Web_Mantenimiento.Models.Tablero;

public class EquipoPorEstadoViewModel
{
    public int IdEstado { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}

public class OrdenPorPrioridadViewModel
{
    public string Prioridad { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Activas { get; set; }
}

public class RepuestoBajoMinimoItemViewModel
{
    public string CodigoRepuesto { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
    public int Deficit { get; set; }
    public decimal CostoUnitario { get; set; }
}

public class RepuestosBajoMinimoResumenViewModel
{
    public int TotalCriticos { get; set; }
    public List<RepuestoBajoMinimoItemViewModel> Items { get; set; } = new();
}

public class TableroResumenViewModel
{
    public List<EquipoPorEstadoViewModel> EquiposPorEstado { get; set; } = new();
    public List<OrdenPorPrioridadViewModel> OrdenesPorPrioridad { get; set; } = new();
    public RepuestosBajoMinimoResumenViewModel RepuestosBajoMinimo { get; set; } = new();
    public string? Error { get; set; }
}
