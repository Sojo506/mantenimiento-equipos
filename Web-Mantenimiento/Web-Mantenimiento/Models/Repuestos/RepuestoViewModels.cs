using System.ComponentModel.DataAnnotations;
using Web_Mantenimiento.Models.Comun;

namespace Web_Mantenimiento.Models.Repuestos;

public class RepuestoViewModel
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

public class RepuestoListadoViewModel
{
    public List<RepuestoViewModel> Repuestos { get; set; } = new();
    public PaginacionViewModel Paginacion { get; set; } = new();
    public string? Busqueda { get; set; }
    public string? OrdenarPor { get; set; }
    public bool SoloBajoMinimo { get; set; }
    public int TotalBajoMinimo { get; set; }
}

public class CrearRepuestoViewModel
{
    [Required(ErrorMessage = "El código del repuesto es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El código debe tener entre 3 y 50 caracteres.")]
    [RegularExpression(@"^[A-Z0-9]+(-[A-Z0-9]+)*$",
        ErrorMessage = "Use solo mayúsculas, números y guiones (ej. REP-001).")]
    [Display(Name = "Código de repuesto")]
    public string CodigoRepuesto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del repuesto es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Range(0, 100000, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    [Range(0, 10000, ErrorMessage = "El stock mínimo no puede ser negativo.")]
    [Display(Name = "Stock mínimo")]
    public int StockMinimo { get; set; }

    [Range(0.01, 1000000.00, ErrorMessage = "El costo unitario debe ser mayor a 0.")]
    [Display(Name = "Costo unitario")]
    public decimal CostoUnitario { get; set; }

    [Range(1, 2, ErrorMessage = "Seleccione un estado válido.")]
    [Display(Name = "Estado")]
    public int IdEstado { get; set; } = 1;
}

public class ActualizarRepuestoViewModel
{
    [Display(Name = "Código de repuesto")]
    public string CodigoRepuesto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del repuesto es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Range(0, 100000, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    [Range(0, 10000, ErrorMessage = "El stock mínimo no puede ser negativo.")]
    [Display(Name = "Stock mínimo")]
    public int StockMinimo { get; set; }

    [Range(0.01, 1000000.00, ErrorMessage = "El costo unitario debe ser mayor a 0.")]
    [Display(Name = "Costo unitario")]
    public decimal CostoUnitario { get; set; }

    [Range(1, 2, ErrorMessage = "Seleccione un estado válido.")]
    [Display(Name = "Estado")]
    public int IdEstado { get; set; }

    [Required(ErrorMessage = "El token de concurrencia es obligatorio.")]
    public string RowVersion { get; set; } = string.Empty;
}
