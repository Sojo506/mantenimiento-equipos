namespace Abstracciones.DTOs.Equipos;

/// <summary>
/// DTO de salida para transferir información de un equipo.
/// </summary>
public class EquipoDto
{
    public string CodigoEquipo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int IdTipoEquipo { get; set; }
    public string TipoEquipoNombre { get; set; } = string.Empty;
    public int IdMarca { get; set; }
    public string MarcaNombre { get; set; } = string.Empty;
    public int IdModelo { get; set; }
    public string ModeloNombre { get; set; } = string.Empty;
    public int IdUbicacion { get; set; }
    public string UbicacionNombre { get; set; } = string.Empty;
    public DateTime FechaAdquisicion { get; set; }
    public int IdEstado { get; set; }
    public string EstadoNombre { get; set; } = string.Empty;
}
