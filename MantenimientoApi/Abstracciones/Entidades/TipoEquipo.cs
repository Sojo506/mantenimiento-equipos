namespace Abstracciones.Entidades;

/// <summary>
/// Catálogo de tipos de equipo (TIPO_EQUIPO_TB).
/// </summary>
public class TipoEquipo
{
    public int IdTipoEquipo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int IdEstado { get; set; }

    public Estado? Estado { get; set; }
    public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
}
