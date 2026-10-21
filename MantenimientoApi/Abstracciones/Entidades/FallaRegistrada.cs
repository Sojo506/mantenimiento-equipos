namespace Abstracciones.Entidades;

/// <summary>
/// Representa una falla registrada asociada a un equipo y a una orden de trabajo (FALLA_REGISTRADA_TB).
/// </summary>
public class FallaRegistrada
{
    public int Id { get; set; }
    public string CodigoEquipo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public int IdTipoGravedad { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int NumeroOrden { get; set; }

    public Equipo? Equipo { get; set; }
    public TipoGravedad? TipoGravedad { get; set; }
    public OrdenDeTrabajo? OrdenDeTrabajo { get; set; }
}
