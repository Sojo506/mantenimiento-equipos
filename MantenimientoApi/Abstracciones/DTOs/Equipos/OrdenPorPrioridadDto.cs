namespace Abstracciones.DTOs.Tablero;

public class OrdenPorPrioridadDto
{
    public string Prioridad { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Activas { get; set; }
}