namespace Abstracciones.Entidades;

/// <summary>
/// Catálogo de marcas de equipo (MARCA_TB).
/// </summary>
public class Marca
{
    public int IdMarca { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int IdEstado { get; set; }

    public Estado? Estado { get; set; }
    public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
}
