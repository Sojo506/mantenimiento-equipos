namespace Abstracciones.Entidades;

/// <summary>
/// Catálogo de modelos de equipo (MODELO_TB).
/// </summary>
public class Modelo
{
    public int IdModelo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int IdEstado { get; set; }

    public Estado? Estado { get; set; }
    public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
}
