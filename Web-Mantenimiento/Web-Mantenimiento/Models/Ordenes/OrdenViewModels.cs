using Web_Mantenimiento.Models.Comun;

namespace Web_Mantenimiento.Models.Ordenes;

public class OrdenRepuestoViewModel
{
    public string CodigoRepuesto { get; set; } = string.Empty;
    public string NombreRepuesto { get; set; } = string.Empty;
    public int CantidadUsada { get; set; }
    public decimal CostoAplicado { get; set; }
    public decimal Subtotal => CantidadUsada * CostoAplicado;
}

public class OrdenViewModel
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
    public List<OrdenRepuestoViewModel> Repuestos { get; set; } = new();
}

public class BitacoraOrdenViewModel
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

public class OrdenListadoViewModel
{
    public string Titulo { get; set; } = "Órdenes de Trabajo";
    public string Subtitulo { get; set; } = "Seguimiento y gestión de órdenes de mantenimiento";
    public string AccionActual { get; set; } = "Index";
    public List<OrdenViewModel> Ordenes { get; set; } = new();
    public PaginacionViewModel Paginacion { get; set; } = new();
    public string? Busqueda { get; set; }
    public string? OrdenarPor { get; set; }
    public int? IdEstado { get; set; }
    public string? Prioridad { get; set; }
}
