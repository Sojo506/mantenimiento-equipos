namespace Abstracciones.Entidades;

/// <summary>
/// Bitácora inmutable de cambios de estado e historial de órdenes de trabajo (BITACORA_ORDEN_TB).
/// </summary>
public class BitacoraOrden
{
    public int IdBitacora { get; set; }
    public int Numero { get; set; }
    public string Identificacion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string EstadoAnterior { get; set; } = string.Empty;
    public string EstadoNuevo { get; set; } = string.Empty;
    public string Comentario { get; set; } = string.Empty;

    public OrdenDeTrabajo? OrdenDeTrabajo { get; set; }
    public Usuario? Usuario { get; set; }
}
