namespace Abstracciones.Entidades;

/// <summary>
/// Catálogo de tipos de gravedad para fallas registradas (TIPO_GRAVEDAD_TB).
/// </summary>
public class TipoGravedad
{
    public int IdTipoGravedad { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int IdEstado { get; set; }

    public Estado? Estado { get; set; }
    public ICollection<FallaRegistrada> FallasRegistradas { get; set; } = new List<FallaRegistrada>();
}
