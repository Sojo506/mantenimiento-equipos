namespace Abstracciones.DTOs.Planes;

public class PlanMantenimientoDto
{
    public int IdPlanMantenimiento { get; set; }
    public string CodigoEquipo { get; set; } = string.Empty;
    public string EquipoNombre { get; set; } = string.Empty;

    public int Frecuencia { get; set; }

    public DateTime ProximaFecha { get; set; }
    public int IdEstado { get; set; }
    public string EstadoNombre { get; set; } = string.Empty;

    public bool Vencido { get; set; }
}
