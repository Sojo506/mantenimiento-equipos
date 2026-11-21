using System.Text.Json.Serialization;

namespace Web_Mantenimiento.Models;

public class ApiLoginResponse
{
    [JsonPropertyName("mensaje")]
    public string Mensaje { get; set; } = string.Empty;

    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("tipoToken")]
    public string TipoToken { get; set; } = string.Empty;

    [JsonPropertyName("expiracion")]
    public DateTime Expiracion { get; set; }

    [JsonPropertyName("usuario")]
    public UsuarioSesion Usuario { get; set; } = new();
}

public class UsuarioSesion
{
    [JsonPropertyName("identificacion")]
    public string Identificacion { get; set; } = string.Empty;

    [JsonPropertyName("correo")]
    public string Correo { get; set; } = string.Empty;

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("primerApellido")]
    public string PrimerApellido { get; set; } = string.Empty;

    [JsonPropertyName("segundoApellido")]
    public string SegundoApellido { get; set; } = string.Empty;

    [JsonPropertyName("rol")]
    public string Rol { get; set; } = string.Empty;
}