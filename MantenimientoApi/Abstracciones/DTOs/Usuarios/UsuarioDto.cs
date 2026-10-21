namespace Abstracciones.DTOs.Usuarios;

/// <summary>
/// DTO de salida para transferir información de un usuario sin exponer contraseñas.
/// </summary>
public class UsuarioDto
{
    public string Identificacion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string PrimerApellido { get; set; } = string.Empty;
    public string SegundoApellido { get; set; } = string.Empty;
    public string NombreCompleto => $"{Nombre} {PrimerApellido} {SegundoApellido}".Trim();
    public string Correo { get; set; } = string.Empty;
    public int IdTipoUsuario { get; set; }
    public string RolNombre { get; set; } = string.Empty;
    public int? IdEspecialidad { get; set; }
    public string? EspecialidadNombre { get; set; }
    public bool Disponibilidad { get; set; }
    public int IdEstado { get; set; }
    public string EstadoNombre { get; set; } = string.Empty;
}
