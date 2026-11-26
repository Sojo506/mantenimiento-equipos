using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Web_Mantenimiento.Models.Comun;

namespace Web_Mantenimiento.Services.Base;

public abstract class ApiClientBase
{
    protected static readonly JsonSerializerOptions JsonOpciones = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected readonly HttpClient HttpClient;

    protected ApiClientBase(IHttpClientFactory factory)
    {
        HttpClient = factory.CreateClient("MantenimientoApi");
    }

    protected Task<(T? Datos, string? Error)> GetAsync<T>(
        string url, CancellationToken ct = default) =>
        EjecutarAsync(async () =>
        {
            var resp = await HttpClient.GetAsync(url, ct);
            return await LeerRespuestaAsync<T>(resp, ct);
        });

    protected Task<(ResultadoPaginado<T>? Datos, string? Error)> GetPaginadoAsync<T>(
        string url, CancellationToken ct = default) =>
        EjecutarAsync<ResultadoPaginado<T>>(async () =>
        {
            var resp = await HttpClient.GetAsync(url, ct);
            if (!resp.IsSuccessStatusCode)
                return (null, await ObtenerErrorAsync(resp, ct));

            var items = await resp.Content.ReadFromJsonAsync<List<T>>(JsonOpciones, ct)
                ?? new List<T>();

            var meta = new PaginacionMeta();
            if (resp.Headers.TryGetValues("X-Pagination", out var valores))
            {
                var raw = valores.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(raw))
                    meta = JsonSerializer.Deserialize<PaginacionMeta>(raw, JsonOpciones) ?? meta;
            }

            return (new ResultadoPaginado<T> { Items = items, Paginacion = meta }, null);
        });

    protected Task<(TRes? Datos, string? Error)> PostAsync<TReq, TRes>(
        string url, TReq cuerpo, CancellationToken ct = default) =>
        EjecutarAsync(async () =>
        {
            var resp = await HttpClient.PostAsJsonAsync(url, cuerpo, JsonOpciones, ct);
            return await LeerRespuestaAsync<TRes>(resp, ct);
        });

    protected Task<(bool Datos, string? Error)> PostAsync<TReq>(
        string url, TReq cuerpo, CancellationToken ct = default) =>
        EjecutarAsync(async () =>
        {
            var resp = await HttpClient.PostAsJsonAsync(url, cuerpo, JsonOpciones, ct);
            return resp.IsSuccessStatusCode
                ? (true, null)
                : (false, await ObtenerErrorAsync(resp, ct));
        });

    protected Task<(TRes? Datos, string? Error)> PutAsync<TReq, TRes>(
        string url, TReq cuerpo, string? ifMatch = null, CancellationToken ct = default) =>
        EjecutarAsync(async () =>
        {
            using var req = CrearPutRequest(url, cuerpo, ifMatch);
            var resp = await HttpClient.SendAsync(req, ct);
            return await LeerRespuestaAsync<TRes>(resp, ct);
        });

    protected Task<(bool Datos, string? Error)> PutAsync<TReq>(
        string url, TReq cuerpo, string? ifMatch = null, CancellationToken ct = default) =>
        EjecutarAsync(async () =>
        {
            using var req = CrearPutRequest(url, cuerpo, ifMatch);
            var resp = await HttpClient.SendAsync(req, ct);
            return resp.IsSuccessStatusCode
                ? (true, null)
                : (false, await ObtenerErrorAsync(resp, ct));
        });

    protected Task<(bool Datos, string? Error)> DeleteAsync(
        string url, CancellationToken ct = default) =>
        EjecutarAsync(async () =>
        {
            var resp = await HttpClient.DeleteAsync(url, ct);
            return resp.IsSuccessStatusCode
                ? (true, null)
                : (false, await ObtenerErrorAsync(resp, ct));
        });

    protected async Task<RespuestaApi<TRes>> EnviarConEstadoAsync<TReq, TRes>(
        HttpMethod metodo, string url, TReq cuerpo, string? ifMatch = null, CancellationToken ct = default)
    {
        try
        {
            using var req = new HttpRequestMessage(metodo, url)
            {
                Content = JsonContent.Create(cuerpo, options: JsonOpciones)
            };
            if (!string.IsNullOrWhiteSpace(ifMatch))
                req.Headers.TryAddWithoutValidation("If-Match", $"\"{ifMatch.Trim('"')}\"");

            var resp = await HttpClient.SendAsync(req, ct);
            if (!resp.IsSuccessStatusCode)
            {
                return new RespuestaApi<TRes>
                {
                    Error = await ObtenerErrorAsync(resp, ct),
                    CodigoEstado = resp.StatusCode
                };
            }

            var datos = resp.StatusCode == HttpStatusCode.NoContent
                ? default
                : await resp.Content.ReadFromJsonAsync<TRes>(JsonOpciones, ct);

            return new RespuestaApi<TRes> { Datos = datos, CodigoEstado = resp.StatusCode };
        }
        catch (HttpRequestException)
        {
            return new RespuestaApi<TRes> { Error = "No fue posible conectar con la API." };
        }
        catch (JsonException)
        {
            return new RespuestaApi<TRes> { Error = "La API devolvió una respuesta inválida." };
        }
    }

    private static HttpRequestMessage CrearPutRequest<TReq>(
        string url, TReq cuerpo, string? ifMatch)
    {
        var req = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = JsonContent.Create(cuerpo, options: JsonOpciones)
        };
        if (!string.IsNullOrWhiteSpace(ifMatch))
            req.Headers.TryAddWithoutValidation("If-Match", ifMatch);

        return req;
    }

    private static async Task<(T? Datos, string? Error)> LeerRespuestaAsync<T>(
        HttpResponseMessage resp, CancellationToken ct)
    {
        if (!resp.IsSuccessStatusCode)
            return (default, await ObtenerErrorAsync(resp, ct));

        if (resp.StatusCode == HttpStatusCode.NoContent)
            return (default, null);

        var datos = await resp.Content.ReadFromJsonAsync<T>(JsonOpciones, ct);
        return (datos, null);
    }

    private static async Task<(T? Datos, string? Error)> EjecutarAsync<T>(
        Func<Task<(T? Datos, string? Error)>> accion)
    {
        try
        {
            return await accion();
        }
        catch (HttpRequestException)
        {
            return (default, "No fue posible conectar con la API.");
        }
        catch (JsonException)
        {
            return (default, "La API devolvió una respuesta inválida.");
        }
    }

    protected static async Task<string> ObtenerErrorAsync(
        HttpResponseMessage resp, CancellationToken ct = default)
    {
        var contenido = await resp.Content.ReadAsStringAsync(ct);

        try
        {
            using var doc = JsonDocument.Parse(contenido);
            var raiz = doc.RootElement;

            if (raiz.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
            {
                var mensajes = errors.EnumerateObject()
                    .SelectMany(p => p.Value.EnumerateArray().Select(v => v.GetString()))
                    .Where(m => !string.IsNullOrWhiteSpace(m));
                var unido = string.Join(" ", mensajes);
                if (!string.IsNullOrWhiteSpace(unido))
                    return unido;
            }

            if (raiz.TryGetProperty("detail", out var detail) && !string.IsNullOrWhiteSpace(detail.GetString()))
                return LimpiarMensajeRegla(detail.GetString()!);

            if (raiz.TryGetProperty("mensaje", out var mensaje) && !string.IsNullOrWhiteSpace(mensaje.GetString()))
                return LimpiarMensajeRegla(mensaje.GetString()!);

            if (raiz.TryGetProperty("title", out var title) && !string.IsNullOrWhiteSpace(title.GetString()))
                return LimpiarMensajeRegla(title.GetString()!);
        }
        catch (JsonException)
        {
            // La respuesta no contiene JSON válido.
        }

        if (resp.StatusCode == HttpStatusCode.Conflict)
            return "Conflicto de concurrencia o estado. Recargue la información e intente nuevamente.";

        return $"Error en la API. Código HTTP: {(int)resp.StatusCode}.";
    }

    private static string LimpiarMensajeRegla(string texto) =>
        System.Text.RegularExpressions.Regex.Replace(texto, @"\s*\(RN-\d{2}\)", "");
}
