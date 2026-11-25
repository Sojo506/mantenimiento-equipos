using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace API.Tests.Infraestructura;

public static class ClienteApi
{
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public const string CorreoAdmin = "admin@mantenimiento.com";
    public const string CorreoSupervisor = "supervisor@mantenimiento.com";
    public const string CorreoTecnico = "tecnico1@mantenimiento.com";
    public const string IdSupervisor = "USR-002";
    public const string IdTecnico1 = "USR-003";
    public const string IdTecnico2 = "USR-004";
    public const string IdSolicitante = "USR-005";

    private static readonly Dictionary<string, string> Contrasenas = new()
    {
        [CorreoAdmin] = "Admin123!",
        [CorreoSupervisor] = "Super123!",
        [CorreoTecnico] = "Tecnico123!"
    };

    public static async Task<HttpClient> CrearClienteAutenticadoAsync(FabricaApiPruebas fabrica, string correo)
    {
        var cliente = fabrica.CreateClient();
        var respuesta = await cliente.PostAsJsonAsync("api/auth/login", new { Correo = correo, Password = Contrasenas[correo] });
        respuesta.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync());
        var token = doc.RootElement.GetProperty("token").GetString();
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return cliente;
    }

    public static async Task<JsonElement> LeerJsonAsync(HttpResponseMessage respuesta)
    {
        var contenido = await respuesta.Content.ReadAsStringAsync();
        return JsonDocument.Parse(contenido).RootElement.Clone();
    }
}
