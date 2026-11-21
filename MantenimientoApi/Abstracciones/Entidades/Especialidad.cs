using Abstracciones.Entidades;
using System.Text.Json.Serialization;

public class Especialidad
{
    public int IdEspecialidad { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int IdEstado { get; set; }

    public Estado? Estado { get; set; }

    [JsonIgnore]
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}