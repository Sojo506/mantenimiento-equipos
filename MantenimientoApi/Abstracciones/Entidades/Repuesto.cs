namespace Abstracciones.Entidades;

/// <summary>
/// Representa un repuesto o insumo en el inventario (REPUESTO_TB).
/// Incluye token de concurrencia optimista para evitar sobreescrituras en compras/consumos simultáneos.
/// </summary>
public class Repuesto
{
    public string CodigoRepuesto { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
    public decimal CostoUnitario { get; set; }
    public int IdEstado { get; set; }

    /// <summary>
    /// Token de concurrencia optimista gestionado por SQL Server.
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Estado? Estado { get; set; }
    public ICollection<OrdenRepuesto> OrdenesRepuestos { get; set; } = new List<OrdenRepuesto>();
}
