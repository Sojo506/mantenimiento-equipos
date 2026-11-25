namespace Abstracciones.DTOs.Equipos;

/// <summary>
/// Resultado de la predicción de ML.NET sobre la probabilidad de falla de un equipo en los próximos 30 días.
/// </summary>
public class PrediccionFallaEquipoDto
{
    public string CodigoEquipo { get; set; } = string.Empty;
    public string EquipoNombre { get; set; } = string.Empty;
    public string EstadoActual { get; set; } = string.Empty;
    public float ProbabilidadFalla30Dias { get; set; }
    public decimal PorcentajeRiesgo => Math.Round((decimal)ProbabilidadFalla30Dias * 100m, 1);
    public bool PrediceFalla { get; set; }
    public string NivelRiesgo { get; set; } = "Bajo";
    public int FallasUltimos6Meses { get; set; }
    public int TotalOrdenesCorrectivas { get; set; }
    public float AntiguedadMeses { get; set; }
    public float HorasMantenimientoAcumuladas { get; set; }
    public string Recomendacion { get; set; } = string.Empty;
}
