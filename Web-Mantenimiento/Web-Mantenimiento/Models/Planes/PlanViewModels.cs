using System.ComponentModel.DataAnnotations;
using Web_Mantenimiento.Models.Comun;

namespace Web_Mantenimiento.Models.Planes;

public class PlanViewModel
{
    public int IdPlanMantenimiento { get; set; }
    public string CodigoEquipo { get; set; } = string.Empty;
    public string EquipoNombre { get; set; } = string.Empty;
    public int Frecuencia { get; set; }
    public DateTime ProximaFecha { get; set; }
    public int IdEstado { get; set; }
    public string EstadoNombre { get; set; } = string.Empty;
    public bool Vencido { get; set; }

    public int DiasParaProxima => (int)Math.Floor((ProximaFecha.Date - DateTime.Today).TotalDays);
}

public class CrearPlanViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un equipo.")]
    [Display(Name = "Equipo")]
    public string CodigoEquipo { get; set; } = string.Empty;

    [Range(1, 365, ErrorMessage = "La frecuencia debe estar entre 1 y 365 días.")]
    [Display(Name = "Frecuencia (días)")]
    public int Frecuencia { get; set; } = 30;

    [Required(ErrorMessage = "La próxima fecha es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Próxima fecha de mantenimiento")]
    public DateTime? ProximaFecha { get; set; } = DateTime.Today.AddDays(30);
}

public class EditarPlanViewModel
{
    public int IdPlanMantenimiento { get; set; }

    [Display(Name = "Equipo")]
    public string CodigoEquipo { get; set; } = string.Empty;

    public string EquipoNombre { get; set; } = string.Empty;

    [Range(1, 365, ErrorMessage = "La frecuencia debe estar entre 1 y 365 días.")]
    [Display(Name = "Frecuencia (días)")]
    public int Frecuencia { get; set; }

    [Required(ErrorMessage = "La próxima fecha es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Próxima fecha de mantenimiento")]
    public DateTime? ProximaFecha { get; set; }

    [Range(1, 2, ErrorMessage = "Seleccione un estado válido.")]
    [Display(Name = "Estado")]
    public int IdEstado { get; set; } = 1;
}

public class PlanListadoViewModel
{
    public List<PlanViewModel> Planes { get; set; } = new();
    public PaginacionViewModel Paginacion { get; set; } = new();
    public string? Busqueda { get; set; }
    public string? OrdenarPor { get; set; }
    public int? IdEstado { get; set; }
    public bool SoloVencidos { get; set; }
}

public class ResultadoProcesoViewModel
{
    public int OrdenesEscaladas { get; set; }
    public int OrdenesPreventivasGeneradas { get; set; }
    public List<int> NumerosOrdenesEscaladas { get; set; } = new();
    public List<int> NumerosOrdenesGeneradas { get; set; } = new();
}
