namespace Abstracciones.Entidades;

/// <summary>
/// Tabla intermedia con atributos para la relación muchos a muchos entre
/// Orden de Trabajo y Repuestos utilizados (ORDEN_REPUESTO_TB).
/// </summary>
public class OrdenRepuesto
{
    public int Numero { get; set; }
    public string CodigoRepuesto { get; set; } = string.Empty;
    public int CantidadUsada { get; set; }
    public decimal CostoAplicado { get; set; }

    public OrdenDeTrabajo? OrdenDeTrabajo { get; set; }
    public Repuesto? Repuesto { get; set; }
}
