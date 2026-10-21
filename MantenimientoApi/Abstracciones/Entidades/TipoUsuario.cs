namespace Abstracciones.Entidades;

/// <summary>
/// Catálogo de roles o tipos de usuario (TIPO_USUARIO_TB).
/// </summary>
public class TipoUsuario
{
    public int IdTipoUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int IdEstado { get; set; }

    public Estado? Estado { get; set; }
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
