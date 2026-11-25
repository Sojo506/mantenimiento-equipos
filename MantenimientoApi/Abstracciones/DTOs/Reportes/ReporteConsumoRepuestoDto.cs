namespace Abstracciones.DTOs.Reportes;

/// <summary>
/// DTO con el resultado del procedimiento almacenado sp_ReporteConsumoRepuestosPorPeriodo.
/// </summary>
public class ReporteConsumoRepuestoDto
{
    public string CodigoRepuesto { get; set; } = string.Empty;
    public string NombreRepuesto { get; set; } = string.Empty;
    public int CantidadTotalConsumida { get; set; }
    public decimal CostoTotalAcumulado { get; set; }
    public int NumeroDeOrdenes { get; set; }
}

/// <summary>
/// DTO para el reporte consolidado de órdenes de trabajo por estado, prioridad, tiempos y costos.
/// </summary>
public class ReporteOrdenesEstadoDto
{
    public int NumeroOrden { get; set; }
    public string CodigoEquipo { get; set; } = string.Empty;
    public string EquipoNombre { get; set; } = string.Empty;
    public string TipoOrden { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string TecnicoAsignado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaLimite { get; set; }
    public decimal TiempoInvertidoHoras { get; set; }
    public decimal CostoRepuestos { get; set; }
}

/// <summary>
/// DTO para el reporte de equipos críticos y reincidencia de fallas (RN-03).
/// </summary>
public class ReporteEquipoCriticoDto
{
    public string CodigoEquipo { get; set; } = string.Empty;
    public string NombreEquipo { get; set; } = string.Empty;
    public string TipoEquipo { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public string EstadoActual { get; set; } = string.Empty;
    public int TotalFallasRegistradas { get; set; }
    public int OrdenesAbiertas { get; set; }
    public int OrdenesCerradas { get; set; }
    public decimal HorasMantenimientoTotal { get; set; }
}

/// <summary>
/// DTO para el reporte de productividad y carga operativa por técnico (RN-07).
/// </summary>
public class ReporteProductividadTecnicoDto
{
    public string Identificacion { get; set; } = string.Empty;
    public string NombreTecnico { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public bool Disponible { get; set; }
    public int OrdenesEnProceso { get; set; }
    public int OrdenesAsignadas { get; set; }
    public int OrdenesCerradas { get; set; }
    public decimal HorasTotalesRegistradas { get; set; }
}

