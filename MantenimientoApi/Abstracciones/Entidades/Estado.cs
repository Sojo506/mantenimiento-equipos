namespace Abstracciones.Entidades;

/// <summary>
/// Entidad de catálogo para los estados del sistema (ESTADO_TB).
/// </summary>
public class Estado
{
    public int IdEstado { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
