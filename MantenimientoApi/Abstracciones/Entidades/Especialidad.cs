namespace Abstracciones.Entidades;

/// <summary>
/// Especialidad técnica de los usuarios técnicos (ESPECIALIDAD_TB).
/// </summary>
public class Especialidad
{
    public int IdEspecialidad { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int IdEstado { get; set; }

    public Estado? Estado { get; set; }
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
