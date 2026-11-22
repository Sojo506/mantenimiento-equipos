namespace Web_Mantenimiento.Models.Comun;

public class PaginacionMeta
{
    public int TotalItems { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}

public class PaginacionViewModel
{
    public PaginacionMeta Meta { get; set; } = new();
    public string? Busqueda { get; set; }
    public string? OrdenarPor { get; set; }
    public string Accion { get; set; } = "Index";
    public string? Controlador { get; set; }
}

public class ResultadoPaginado<T>
{
    public List<T> Items { get; set; } = new();
    public PaginacionMeta Paginacion { get; set; } = new();
}
