namespace Abstracciones.Entidades;

/// <summary>
/// Representa un equipo o activo de la empresa sujeto a mantenimiento (EQUIPO_TB).
/// </summary>
public class Equipo
{
    public string CodigoEquipo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int IdTipoEquipo { get; set; }
    public int IdMarca { get; set; }
    public int IdModelo { get; set; }
    public int IdUbicacion { get; set; }
    public DateTime FechaAdquisicion { get; set; }
    public int IdEstado { get; set; }

    public TipoEquipo? TipoEquipo { get; set; }
    public Marca? Marca { get; set; }
    public Modelo? Modelo { get; set; }
    public Ubicacion? Ubicacion { get; set; }
    public Estado? Estado { get; set; }

    public ICollection<OrdenDeTrabajo> OrdenesDeTrabajo { get; set; } = new List<OrdenDeTrabajo>();
    public ICollection<FallaRegistrada> FallasRegistradas { get; set; } = new List<FallaRegistrada>();
    public ICollection<PlanDeMantenimiento> PlanesDeMantenimiento { get; set; } = new List<PlanDeMantenimiento>();
}
