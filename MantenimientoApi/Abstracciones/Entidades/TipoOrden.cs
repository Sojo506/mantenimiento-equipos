namespace Abstracciones.Entidades;

/// <summary>
/// Catálogo de tipos de órdenes de trabajo (TIPO_ORDEN_TB).
/// </summary>
public class TipoOrden
{
    public int IdTipoOrden { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int IdEstado { get; set; }

    public Estado? Estado { get; set; }
    public ICollection<OrdenDeTrabajo> OrdenesDeTrabajo { get; set; } = new List<OrdenDeTrabajo>();
}
