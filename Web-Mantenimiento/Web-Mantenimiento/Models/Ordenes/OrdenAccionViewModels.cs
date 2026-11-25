using System.ComponentModel.DataAnnotations;

namespace Web_Mantenimiento.Models.Ordenes;

public class CrearOrdenViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un equipo.")]
    [Display(Name = "Equipo")]
    public string CodigoEquipo { get; set; } = string.Empty;

    [Display(Name = "Técnico asignado (opcional)")]
    public string? IdentificacionTecnico { get; set; }

    [Range(1, 2, ErrorMessage = "Debe seleccionar un tipo de orden.")]
    [Display(Name = "Tipo de orden")]
    public int IdTipoOrden { get; set; } = 2;

    [Required(ErrorMessage = "La prioridad es obligatoria.")]
    [RegularExpression("^(Baja|Media|Alta|Critica)$",
        ErrorMessage = "La prioridad debe ser Baja, Media, Alta o Critica.")]
    public string Prioridad { get; set; } = "Media";

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "La descripción debe tener entre 5 y 500 caracteres.")]
    [Display(Name = "Descripción del trabajo o falla")]
    public string Descripcion { get; set; } = string.Empty;
}

public class CambiarEstadoOrdenViewModel
{
    public int Numero { get; set; }

    [Range(7, 12, ErrorMessage = "Seleccione un estado válido.")]
    [Display(Name = "Nuevo estado")]
    public int NuevoEstado { get; set; }

    [Required(ErrorMessage = "El comentario es obligatorio.")]
    [StringLength(500, MinimumLength = 3, ErrorMessage = "El comentario debe tener entre 3 y 500 caracteres.")]
    public string Comentario { get; set; } = string.Empty;

    [Range(0, 1000, ErrorMessage = "El tiempo invertido debe ser mayor o igual a 0.")]
    [Display(Name = "Tiempo invertido final (horas)")]
    public decimal? TiempoInvertidoFinal { get; set; }

    [Display(Name = "Cierre sin repuestos")]
    public bool SinRepuestos { get; set; }

    [Display(Name = "Justificación sin repuestos")]
    public string? JustificacionSinRepuestos { get; set; }

    [Required]
    public string RowVersion { get; set; } = string.Empty;
}

public class AsignarRepuestoOrdenViewModel
{
    public int Numero { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un repuesto.")]
    [Display(Name = "Repuesto")]
    public string CodigoRepuesto { get; set; } = string.Empty;

    [Range(1, 1000, ErrorMessage = "La cantidad debe ser mayor a 0.")]
    public int Cantidad { get; set; } = 1;

    public string? RowVersion { get; set; }
}

public class AsignarTecnicoOrdenViewModel
{
    public int Numero { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un técnico.")]
    [Display(Name = "Técnico")]
    public string IdentificacionTecnico { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "El comentario no puede superar 500 caracteres.")]
    public string? Comentario { get; set; }

    [Required]
    public string RowVersion { get; set; } = string.Empty;

    public string? Volver { get; set; }
}

public class AtenderOrdenViewModel
{
    public int Numero { get; set; }

    [Range(7, 12)]
    public int NuevoEstado { get; set; }

    [Required]
    public string RowVersion { get; set; } = string.Empty;

    public string? Comentario { get; set; }
}
