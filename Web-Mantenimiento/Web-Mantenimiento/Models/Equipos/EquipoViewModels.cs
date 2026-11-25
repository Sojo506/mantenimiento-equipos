using System.ComponentModel.DataAnnotations;
using Web_Mantenimiento.Models.Comun;

namespace Web_Mantenimiento.Models.Equipos;

public class EquipoViewModel
{
    public string CodigoEquipo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int IdTipoEquipo { get; set; }
    public string TipoEquipoNombre { get; set; } = string.Empty;
    public int IdMarca { get; set; }
    public string MarcaNombre { get; set; } = string.Empty;
    public int IdModelo { get; set; }
    public string ModeloNombre { get; set; } = string.Empty;
    public int IdUbicacion { get; set; }
    public string UbicacionNombre { get; set; } = string.Empty;
    public DateTime FechaAdquisicion { get; set; }
    public int IdEstado { get; set; }
    public string EstadoNombre { get; set; } = string.Empty;
}

public class EquipoListadoViewModel
{
    public List<EquipoViewModel> Equipos { get; set; } = new();
    public PaginacionViewModel Paginacion { get; set; } = new();
    public string? Busqueda { get; set; }
    public string? OrdenarPor { get; set; }
}

public class CrearEquipoViewModel
{
    [Required(ErrorMessage = "El código de equipo es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El código debe tener entre 3 y 50 caracteres.")]
    [RegularExpression(@"^[A-Z0-9]+(-[A-Z0-9]+)*$",
        ErrorMessage = "Use solo mayúsculas, números y guiones (ej. EQ-001).")]
    [Display(Name = "Código de equipo")]
    public string CodigoEquipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del equipo es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de equipo.")]
    [Display(Name = "Tipo de equipo")]
    public int IdTipoEquipo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una marca.")]
    [Display(Name = "Marca")]
    public int IdMarca { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un modelo.")]
    [Display(Name = "Modelo")]
    public int IdModelo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una ubicación.")]
    [Display(Name = "Ubicación")]
    public int IdUbicacion { get; set; }

    [Required(ErrorMessage = "La fecha de adquisición es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de adquisición")]
    public DateTime FechaAdquisicion { get; set; } = DateTime.Today;

    [Range(3, 6, ErrorMessage = "Seleccione un estado de equipo válido.")]
    [Display(Name = "Estado")]
    public int IdEstado { get; set; } = 3;
}

public class ActualizarEquipoViewModel
{
    [Display(Name = "Código de equipo")]
    public string CodigoEquipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del equipo es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de equipo.")]
    [Display(Name = "Tipo de equipo")]
    public int IdTipoEquipo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una marca.")]
    [Display(Name = "Marca")]
    public int IdMarca { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un modelo.")]
    [Display(Name = "Modelo")]
    public int IdModelo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una ubicación.")]
    [Display(Name = "Ubicación")]
    public int IdUbicacion { get; set; }

    [Range(3, 6, ErrorMessage = "Seleccione un estado de equipo válido.")]
    [Display(Name = "Estado")]
    public int IdEstado { get; set; }
}

public class PrediccionFallaEquipoViewModel
{
    public string CodigoEquipo { get; set; } = string.Empty;
    public string EquipoNombre { get; set; } = string.Empty;
    public string EstadoActual { get; set; } = string.Empty;
    public float ProbabilidadFalla30Dias { get; set; }
    public decimal PorcentajeRiesgo { get; set; }
    public bool PrediceFalla { get; set; }
    public string NivelRiesgo { get; set; } = "Bajo";
    public int FallasUltimos6Meses { get; set; }
    public int TotalOrdenesCorrectivas { get; set; }
    public float AntiguedadMeses { get; set; }
    public float HorasMantenimientoAcumuladas { get; set; }
    public string Recomendacion { get; set; } = string.Empty;
}
