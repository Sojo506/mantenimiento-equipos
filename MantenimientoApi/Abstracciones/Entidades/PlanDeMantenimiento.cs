namespace Abstracciones.Entidades;

/// <summary>
/// Representa la programación de mantenimientos preventivos para un equipo (PLAN_MANTENIMIENTO_TB).
/// </summary>
public class PlanDeMantenimiento
{
    public int IdPlanMantenimiento { get; set; }
    public string CodigoEquipo { get; set; } = string.Empty;
    public int Frecuencia { get; set; }
    public DateTime ProximaFecha { get; set; }
    public int IdEstado { get; set; }

    public Equipo? Equipo { get; set; }
    public Estado? Estado { get; set; }
}
