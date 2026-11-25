
using System.Net.Http.Json;
using System.Text.Json;
using Web_Mantenimiento.Models;

namespace Web_Mantenimiento.Services;

public class ApiAuthService
{
    private readonly HttpClient _httpClient;

    public ApiAuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(ApiLoginResponse? Respuesta, string? Error)>
        LoginAsync(
            string correo,
            string password,
            CancellationToken cancellationToken = default)
    {
        var datos = new
        {
            Correo = correo,
            Password = password
        };

        var respuesta = await _httpClient.PostAsJsonAsync(
            "api/Auth/login",
            datos,
            cancellationToken);

        var contenido = await respuesta.Content.ReadAsStringAsync(
            cancellationToken);

        if (!respuesta.IsSuccessStatusCode)
        {
            string mensaje = "No fue posible iniciar sesión.";

            try
            {
                using var documento = JsonDocument.Parse(contenido);

                if (documento.RootElement.TryGetProperty(
                    "detail", out var detailProp) && !string.IsNullOrWhiteSpace(detailProp.GetString()))
                {
                    mensaje = detailProp.GetString()!;
                }
                else if (documento.RootElement.TryGetProperty(
                    "mensaje", out var propiedad))
                {
                    mensaje = propiedad.GetString() ?? mensaje;
                }
            }
            catch (JsonException)
            {
                // Se conserva el mensaje genérico.
            }

            return (null, mensaje);
        }

        var resultado = JsonSerializer.Deserialize<ApiLoginResponse>(
            contenido,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (resultado == null ||
            string.IsNullOrWhiteSpace(resultado.Token) ||
            resultado.Usuario == null)
        {
            return (null, "La API devolvió una respuesta de autenticación inválida.");
        }

        return (resultado, null);
    }
}
