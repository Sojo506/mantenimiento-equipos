using System.ComponentModel.DataAnnotations;

namespace Web_Mantenimiento.Models.Fallas;

public class FallaViewModel
{
    public int Id { get; set; }
    public string CodigoEquipo { get; set; } = string.Empty;
    public string EquipoNombre { get; set; } = string.Empty;
    public int EstadoEquipoActual { get; set; }
    public string EstadoEquipoNombre { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public int IdTipoGravedad { get; set; }
    public string GravedadNombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int NumeroOrden { get; set; }
    public bool PasoAEnRiesgoAutomatico { get; set; }
}

public class RegistrarFallaViewModel
{
    [Required(ErrorMessage = "Debe seleccionar el equipo afectado.")]
    [Display(Name = "Equipo afectado")]
    public string CodigoEquipo { get; set; } = string.Empty;

    [Range(1, 4, ErrorMessage = "Seleccione el nivel de gravedad.")]
    [Display(Name = "Nivel de gravedad")]
    public int IdTipoGravedad { get; set; } = 2;

    [Required(ErrorMessage = "La descripción de la falla es obligatoria.")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "La descripción debe tener entre 5 y 500 caracteres.")]
    [Display(Name = "Descripción detallada de la falla")]
    public string Descripcion { get; set; } = string.Empty;
}
