namespace Abstracciones.Entidades;

/// <summary>
/// Entidad principal del sistema que representa una orden de trabajo de mantenimiento (ORDEN_TB).
/// </summary>
public class OrdenDeTrabajo
{
    public int Numero { get; set; }
    public string CodigoEquipo { get; set; } = string.Empty;
    public string IdentificacionSolicitante { get; set; } = string.Empty;
    public string? IdentificacionTecnico { get; set; }
    public int IdTipoOrden { get; set; }
    public string Prioridad { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaLimite { get; set; }
    public decimal TiempoInvertido { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public bool SinRepuestos { get; set; }
    public string? JustificacionSinRepuestos { get; set; }
    public int IdEstado { get; set; }

    /// <summary>
    /// Token de concurrencia optimista gestionado por SQL Server.
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Equipo? Equipo { get; set; }
    public Usuario? Solicitante { get; set; }
    public Usuario? Tecnico { get; set; }
    public TipoOrden? TipoOrden { get; set; }
    public Estado? Estado { get; set; }

    public ICollection<OrdenRepuesto> RepuestosUsados { get; set; } = new List<OrdenRepuesto>();
    public ICollection<FallaRegistrada> FallasRegistradas { get; set; } = new List<FallaRegistrada>();
    public ICollection<BitacoraOrden> Bitacoras { get; set; } = new List<BitacoraOrden>();
}
