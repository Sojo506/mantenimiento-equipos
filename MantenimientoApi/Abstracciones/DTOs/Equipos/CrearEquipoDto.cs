using System.ComponentModel.DataAnnotations;

namespace Abstracciones.DTOs.Equipos;

/// <summary>
/// DTO de entrada para registrar un nuevo equipo.
/// </summary>
public class CrearEquipoDto
{
    [Required(ErrorMessage = "El código de equipo es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El código debe tener entre 3 y 50 caracteres.")]
    [Abstracciones.Validaciones.CodigoFormato]
    public string CodigoEquipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del equipo es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de equipo válido.")]
    public int IdTipoEquipo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una marca válida.")]
    public int IdMarca { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un modelo válido.")]
    public int IdModelo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una ubicación válida.")]
    public int IdUbicacion { get; set; }

    [Required(ErrorMessage = "La fecha de adquisición es requerida.")]
    public DateTime FechaAdquisicion { get; set; }

    [Range(3, 6, ErrorMessage = "El estado del equipo debe ser 3 (Operativo), 4 (EnMantenimiento), 5 (EnRiesgo) o 6 (DadoDeBaja).")]
    public int IdEstado { get; set; } = 3;
}
