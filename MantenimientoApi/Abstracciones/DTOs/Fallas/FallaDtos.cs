using System.ComponentModel.DataAnnotations;
using Abstracciones.Validaciones;

namespace Abstracciones.DTOs.Fallas;

/// <summary>
/// DTO de salida con el detalle de una falla registrada y el estado resultante del equipo.
/// </summary>
public class FallaRegistradaDto
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

/// <summary>
/// DTO de entrada para reportar una nueva falla sobre un equipo (activa validación RN-03).
/// </summary>
public class RegistrarFallaDto
{
    [Required(ErrorMessage = "El código de equipo es obligatorio.")]
    [CodigoFormato]
    public string CodigoEquipo { get; set; } = string.Empty;

    [Range(1, 4, ErrorMessage = "El tipo de gravedad debe estar entre 1 (Baja) y 4 (Critica).")]
    public int IdTipoGravedad { get; set; } = 2;

    [Required(ErrorMessage = "La descripción de la falla es obligatoria.")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "La descripción debe tener entre 5 y 500 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Identificación del usuario que reporta la falla (para generar o asociar la orden correctiva si NumeroOrden es nulo).
    /// </summary>
    public string? IdentificacionSolicitante { get; set; }

    /// <summary>
    /// Número de orden opcional. Si no se indica, se vincula a la orden correctiva activa del equipo o se crea una nueva.
    /// </summary>
    public int? NumeroOrden { get; set; }
}
