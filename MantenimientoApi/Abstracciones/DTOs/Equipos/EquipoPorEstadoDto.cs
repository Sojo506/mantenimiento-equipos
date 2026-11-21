namespace Abstracciones.DTOs.Tablero;

public class EquipoPorEstadoDto
{
    public int IdEstado { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}