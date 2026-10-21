namespace Abstracciones.Entidades;

/// <summary>
/// Representa a un usuario del sistema (USUARIO_TB), unificando administradores,
/// supervisores, solicitantes y técnicos.
/// </summary>
public class Usuario
{
    public string Identificacion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string PrimerApellido { get; set; } = string.Empty;
    public string SegundoApellido { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int IdTipoUsuario { get; set; }
    public int? IdEspecialidad { get; set; }
    public bool Disponibilidad { get; set; } = true;
    public int IdEstado { get; set; }

    public TipoUsuario? TipoUsuario { get; set; }
    public Especialidad? Especialidad { get; set; }
    public Estado? Estado { get; set; }

    public ICollection<OrdenDeTrabajo> OrdenesSolicitadas { get; set; } = new List<OrdenDeTrabajo>();
    public ICollection<OrdenDeTrabajo> OrdenesAsignadas { get; set; } = new List<OrdenDeTrabajo>();
    public ICollection<BitacoraOrden> Bitacoras { get; set; } = new List<BitacoraOrden>();
}
