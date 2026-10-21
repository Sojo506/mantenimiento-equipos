namespace Abstracciones.Configuracion;

/// <summary>
/// Opciones de configuración para los umbrales y plazos de las reglas de negocio (RN-01 a RN-09).
/// Se enlaza a la sección "ReglasNegocio" de appsettings.json bajo el patrón Options.
/// </summary>
public class ReglasNegocioOptions
{
    public const string Seccion = "ReglasNegocio";

    /// <summary>
    /// Horas máximas que una orden crítica puede permanecer sin pasar a 'En Proceso' antes de escalarse (RN-01).
    /// </summary>
    public int MaxHorasCriticaSinAtender { get; set; } = 4;

    /// <summary>
    /// Cantidad de fallas en 6 meses que provocan que un equipo pase automáticamente a 'En Riesgo' (RN-03).
    /// </summary>
    public int MaxFallasParaRiesgo { get; set; } = 3;

    /// <summary>
    /// Cantidad máxima de órdenes simultáneas en estado 'En Proceso' que puede tener un técnico (RN-07).
    /// </summary>
    public int MaxOrdenesEnProcesoPorTecnico { get; set; } = 3;

    /// <summary>
    /// Plazo en horas para resolver órdenes con prioridad Baja (RN-08).
    /// </summary>
    public int PlazoHorasBaja { get; set; } = 240; // 10 días

    /// <summary>
    /// Plazo en horas para resolver órdenes con prioridad Media (RN-08).
    /// </summary>
    public int PlazoHorasMedia { get; set; } = 120; // 5 días

    /// <summary>
    /// Plazo en horas para resolver órdenes con prioridad Alta (RN-08).
    /// </summary>
    public int PlazoHorasAlta { get; set; } = 48; // 2 días

    /// <summary>
    /// Plazo en horas para resolver órdenes con prioridad Crítica (RN-08).
    /// </summary>
    public int PlazoHorasCritica { get; set; } = 24; // 1 día
}
