
using System.Net.Http.Json;
using System.Text.Json;
using Web_Mantenimiento.Models.Usuarios;

namespace Web_Mantenimiento.Services;

public class ApiUsuarioService
{
    private readonly HttpClient _httpClient;

    public ApiUsuarioService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("MantenimientoApi");
    }

    public async Task<(List<UsuarioViewModel>? Datos, string? Error)>
        ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var respuesta = await _httpClient.GetAsync(
                "api/Usuarios", cancellationToken);

            if (!respuesta.IsSuccessStatusCode)
                return (null, await ObtenerErrorAsync(respuesta));

            var usuarios = await respuesta.Content
                .ReadFromJsonAsync<List<UsuarioViewModel>>(
                    cancellationToken: cancellationToken);

            return (usuarios ?? new List<UsuarioViewModel>(), null);
        }
        catch (HttpRequestException)
        {
            return (null, "No fue posible conectar con la API.");
        }
        catch (JsonException)
        {
            return (null, "La API devolvió una respuesta inválida.");
        }
    }

    public async Task<(UsuarioViewModel? Datos, string? Error)>
        ObtenerPorIdentificacionAsync(
            string identificacion,
            CancellationToken cancellationToken = default)
    {
        try
        {
            var respuesta = await _httpClient.GetAsync(
                $"api/Usuarios/buscar/{Uri.EscapeDataString(identificacion)}",
                cancellationToken);

            if (!respuesta.IsSuccessStatusCode)
                return (null, await ObtenerErrorAsync(respuesta));

            var usuario = await respuesta.Content
                .ReadFromJsonAsync<UsuarioViewModel>(
                    cancellationToken: cancellationToken);

            return (usuario, null);
        }
        catch (HttpRequestException)
        {
            return (null, "No fue posible conectar con la API.");
        }
        catch (JsonException)
        {
            return (null, "La API devolvió una respuesta inválida.");
        }
    }

    public async Task<string?> CrearAsync(
        CrearUsuarioViewModel modelo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var respuesta = await _httpClient.PostAsJsonAsync(
                "api/Usuarios", modelo, cancellationToken);

            return respuesta.IsSuccessStatusCode
                ? null
                : await ObtenerErrorAsync(respuesta);
        }
        catch (HttpRequestException)
        {
            return "No fue posible conectar con la API.";
        }
    }

    public async Task<string?> ActualizarAsync(
        string identificacion,
        ActualizarUsuarioViewModel modelo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var datos = new
            {
                modelo.Nombre,
                modelo.PrimerApellido,
                modelo.SegundoApellido,
                modelo.Correo,
                modelo.IdTipoUsuario,
                modelo.IdEspecialidad,
                modelo.IdEstado,
                modelo.Disponibilidad
            };

            var respuesta = await _httpClient.PutAsJsonAsync(
                $"api/Usuarios/{Uri.EscapeDataString(identificacion)}",
                datos,
                cancellationToken);

            return respuesta.IsSuccessStatusCode
                ? null
                : await ObtenerErrorAsync(respuesta);
        }
        catch (HttpRequestException)
        {
            return "No fue posible conectar con la API.";
        }
    }

    public async Task<string?> EliminarAsync(
        string identificacion,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var respuesta = await _httpClient.DeleteAsync(
                $"api/Usuarios/{Uri.EscapeDataString(identificacion)}",
                cancellationToken);

            return respuesta.IsSuccessStatusCode
                ? null
                : await ObtenerErrorAsync(respuesta);
        }
        catch (HttpRequestException)
        {
            return "No fue posible conectar con la API.";
        }
    }

    private static async Task<string> ObtenerErrorAsync(
        HttpResponseMessage respuesta)
    {
        var contenido = await respuesta.Content.ReadAsStringAsync();

        try
        {
            using var documento = JsonDocument.Parse(contenido);
            var raiz = documento.RootElement;

            if (raiz.TryGetProperty("mensaje", out var mensaje))
                return mensaje.GetString() ?? "Ocurrió un error.";

            if (raiz.TryGetProperty("detail", out var detalle))
                return detalle.GetString() ?? "Ocurrió un error.";

            if (raiz.TryGetProperty("title", out var titulo))
                return titulo.GetString() ?? "Ocurrió un error.";
        }
        catch (JsonException)
        {
            // La respuesta no contiene JSON.
        }

        return $"Error en la API. Código HTTP: {(int)respuesta.StatusCode}.";
    }
}
