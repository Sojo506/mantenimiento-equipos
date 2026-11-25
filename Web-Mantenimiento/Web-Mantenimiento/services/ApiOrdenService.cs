using Microsoft.AspNetCore.WebUtilities;
using Web_Mantenimiento.Models.Comun;
using Web_Mantenimiento.Models.Ordenes;
using Web_Mantenimiento.Services.Base;

namespace Web_Mantenimiento.Services;

public class ApiOrdenService : ApiClientBase
{
    public ApiOrdenService(IHttpClientFactory factory) : base(factory)
    {
    }

    public Task<(ResultadoPaginado<OrdenViewModel>? Datos, string? Error)> ObtenerPaginadoAsync(
        string? busqueda = null,
        string? ordenarPor = null,
        int? idEstado = null,
        string? prioridad = null,
        string? identificacionTecnico = null,
        string? identificacionSolicitante = null,
        int pagina = 1,
        int tamanoPagina = 10,
        CancellationToken ct = default,
        bool activas = false,
        bool vencidas = false)
    {
        var p = new Dictionary<string, string?>
        {
            ["pagina"] = Math.Max(1, pagina).ToString(),
            ["tamanoPagina"] = Math.Clamp(tamanoPagina, 1, 50).ToString()
        };

        if (!string.IsNullOrWhiteSpace(busqueda)) p["busqueda"] = busqueda.Trim();
        if (!string.IsNullOrWhiteSpace(ordenarPor)) p["ordenarPor"] = ordenarPor.Trim();
        if (idEstado.HasValue) p["idEstado"] = idEstado.Value.ToString();
        if (!string.IsNullOrWhiteSpace(prioridad)) p["prioridad"] = prioridad.Trim();
        if (!string.IsNullOrWhiteSpace(identificacionTecnico)) p["identificacionTecnico"] = identificacionTecnico.Trim();
        if (!string.IsNullOrWhiteSpace(identificacionSolicitante)) p["identificacionSolicitante"] = identificacionSolicitante.Trim();
        if (activas) p["activas"] = "true";
        if (vencidas) p["vencidas"] = "true";

        var url = QueryHelpers.AddQueryString("api/Ordenes", p);
        return GetPaginadoAsync<OrdenViewModel>(url, ct);
    }

    public Task<(OrdenViewModel? Datos, string? Error)> ObtenerPorNumeroAsync(
        int numero, CancellationToken ct = default) =>
        GetAsync<OrdenViewModel>($"api/Ordenes/{numero}", ct);

    public Task<(OrdenViewModel? Datos, string? Error)> CrearAsync(
        CrearOrdenViewModel modelo, string identificacionSolicitante, CancellationToken ct = default)
    {
        var cuerpo = new
        {
            modelo.CodigoEquipo,
            IdentificacionSolicitante = identificacionSolicitante,
            IdentificacionTecnico = string.IsNullOrWhiteSpace(modelo.IdentificacionTecnico)
                ? null : modelo.IdentificacionTecnico,
            modelo.IdTipoOrden,
            modelo.Prioridad,
            modelo.Descripcion
        };

        return PostAsync<object, OrdenViewModel>("api/Ordenes", cuerpo, ct);
    }

    public async Task<int> ContarAsync(
        int? idEstado = null, string? identificacionTecnico = null,
        bool activas = false, bool vencidas = false, CancellationToken ct = default)
    {
        var (res, _) = await ObtenerPaginadoAsync(
            null, null, idEstado, null, identificacionTecnico, null, 1, 1, ct, activas, vencidas);
        return res?.Paginacion.TotalItems ?? 0;
    }

    public Task<RespuestaApi<OrdenViewModel>> CambiarEstadoAsync(
        CambiarEstadoOrdenViewModel modelo, string identificacionUsuario, CancellationToken ct = default)
    {
        var cuerpo = new
        {
            modelo.NuevoEstado,
            IdentificacionUsuario = identificacionUsuario,
            modelo.Comentario,
            modelo.TiempoInvertidoFinal,
            SinRepuestos = modelo.NuevoEstado == 11 ? (bool?)modelo.SinRepuestos : null,
            modelo.JustificacionSinRepuestos,
            modelo.RowVersion
        };

        return EnviarConEstadoAsync<object, OrdenViewModel>(
            HttpMethod.Put, $"api/Ordenes/{modelo.Numero}/estado", cuerpo, modelo.RowVersion, ct);
    }

    public Task<RespuestaApi<OrdenViewModel>> AsignarRepuestoAsync(
        AsignarRepuestoOrdenViewModel modelo, CancellationToken ct = default)
    {
        var cuerpo = new { modelo.CodigoRepuesto, modelo.Cantidad, modelo.RowVersion };
        return EnviarConEstadoAsync<object, OrdenViewModel>(
            HttpMethod.Post, $"api/Ordenes/{modelo.Numero}/repuestos", cuerpo, modelo.RowVersion, ct);
    }

    public Task<RespuestaApi<OrdenViewModel>> AsignarTecnicoAsync(
        AsignarTecnicoOrdenViewModel modelo, string identificacionUsuario, CancellationToken ct = default)
    {
        var cuerpo = new
        {
            modelo.IdentificacionTecnico,
            IdentificacionUsuario = identificacionUsuario,
            Comentario = string.IsNullOrWhiteSpace(modelo.Comentario) ? null : modelo.Comentario.Trim(),
            modelo.RowVersion
        };
        return EnviarConEstadoAsync<object, OrdenViewModel>(
            HttpMethod.Put, $"api/Ordenes/{modelo.Numero}/asignacion", cuerpo, modelo.RowVersion, ct);
    }

    public Task<(List<BitacoraOrdenViewModel>? Datos, string? Error)> ObtenerHistorialAsync(
        int numero, CancellationToken ct = default) =>
        GetAsync<List<BitacoraOrdenViewModel>>($"api/Ordenes/{numero}/historial", ct);
}
