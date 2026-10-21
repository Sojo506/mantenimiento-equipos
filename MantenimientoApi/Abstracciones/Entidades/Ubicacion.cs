namespace Abstracciones.Entidades;

/// <summary>
/// Catálogo de ubicaciones físicas de los equipos (UBICACION_TB).
/// </summary>
public class Ubicacion
{
    public int IdUbicacion { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int IdEstado { get; set; }

    public Estado? Estado { get; set; }
    public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
}
