namespace Abstracciones.DTOs.Planes;

public class ResultadoProcesoAutomaticoDto
{
    public int OrdenesEscaladas { get; set; }
    public int OrdenesPreventivasGeneradas { get; set; }
    public List<int> NumerosOrdenesEscaladas { get; set; } = new();
    public List<int> NumerosOrdenesGeneradas { get; set; } = new();
}
