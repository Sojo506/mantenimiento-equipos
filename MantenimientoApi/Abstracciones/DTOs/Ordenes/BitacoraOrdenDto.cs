namespace Abstracciones.DTOs.Ordenes;

/// <summary>
/// DTO de salida para transferir el historial inmutable de auditoría y bitácora de una orden de trabajo.
/// </summary>
public class BitacoraOrdenDto
{
    public int IdBitacora { get; set; }
    public int Numero { get; set; }
    public string Identificacion { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string EstadoAnterior { get; set; } = string.Empty;
    public string EstadoNuevo { get; set; } = string.Empty;
    public string Comentario { get; set; } = string.Empty;
}
