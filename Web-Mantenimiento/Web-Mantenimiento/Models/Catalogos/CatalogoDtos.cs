namespace Web_Mantenimiento.Models.Catalogos;

public class EstadoDto
{
    public int IdEstado { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class TipoUsuarioDto
{
    public int IdTipoUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class TipoEquipoDto
{
    public int IdTipoEquipo { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class MarcaDto
{
    public int IdMarca { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class ModeloDto
{
    public int IdModelo { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class UbicacionDto
{
    public int IdUbicacion { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class EspecialidadDto
{
    public int IdEspecialidad { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class TipoOrdenDto
{
    public int IdTipoOrden { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class TipoGravedadDto
{
    public int IdTipoGravedad { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
