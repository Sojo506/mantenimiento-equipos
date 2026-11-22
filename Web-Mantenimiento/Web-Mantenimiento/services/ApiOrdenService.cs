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
        CancellationToken ct = default)
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

    public Task<(OrdenViewModel? Datos, string? Error)> CambiarEstadoAsync(
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

        var ifMatch = string.IsNullOrWhiteSpace(modelo.RowVersion) ? null : $"\"{modelo.RowVersion}\"";
        return PutAsync<object, OrdenViewModel>($"api/Ordenes/{modelo.Numero}/estado", cuerpo, ifMatch, ct);
    }

    public Task<(OrdenViewModel? Datos, string? Error)> AsignarRepuestoAsync(
        AsignarRepuestoOrdenViewModel modelo, CancellationToken ct = default)
    {
        var cuerpo = new { modelo.CodigoRepuesto, modelo.Cantidad };
        return PostAsync<object, OrdenViewModel>($"api/Ordenes/{modelo.Numero}/repuestos", cuerpo, ct);
    }

    public Task<(List<BitacoraOrdenViewModel>? Datos, string? Error)> ObtenerHistorialAsync(
        int numero, CancellationToken ct = default) =>
        GetAsync<List<BitacoraOrdenViewModel>>($"api/Ordenes/{numero}/historial", ct);
}
