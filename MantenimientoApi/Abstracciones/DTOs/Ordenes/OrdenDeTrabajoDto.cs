namespace Abstracciones.DTOs.Ordenes;

/// <summary>
/// DTO de salida para transferir información de una orden de trabajo.
/// </summary>
public class OrdenDeTrabajoDto
{
    public int Numero { get; set; }
    public string CodigoEquipo { get; set; } = string.Empty;
    public string EquipoNombre { get; set; } = string.Empty;
    public string IdentificacionSolicitante { get; set; } = string.Empty;
    public string SolicitanteNombre { get; set; } = string.Empty;
    public string? IdentificacionTecnico { get; set; }
    public string? TecnicoNombre { get; set; }
    public int IdTipoOrden { get; set; }
    public string TipoOrdenNombre { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaLimite { get; set; }
    public decimal TiempoInvertido { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public bool SinRepuestos { get; set; }
    public string? JustificacionSinRepuestos { get; set; }
    public int IdEstado { get; set; }
    public string EstadoNombre { get; set; } = string.Empty;
    public string RowVersion { get; set; } = string.Empty;

    public List<OrdenRepuestoDto> Repuestos { get; set; } = new();
}
