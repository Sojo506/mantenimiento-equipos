namespace Abstracciones.Enums;

/// <summary>
/// Estados posibles en el ciclo de vida de una orden de trabajo (ESTADO_TB).
/// </summary>
public enum EstadoOrdenEnum
{
    Abierta = 7,
    Asignada = 8,
    EnProceso = 9,
    EnEsperaDeRepuesto = 10,
    Cerrada = 11,
    Escalada = 12
}
