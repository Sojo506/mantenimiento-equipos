using Microsoft.AspNetCore.WebUtilities;
using Web_Mantenimiento.Models.Comun;
using Web_Mantenimiento.Models.Repuestos;
using Web_Mantenimiento.Services.Base;

namespace Web_Mantenimiento.Services;

public class ApiRepuestoService : ApiClientBase
{
    public ApiRepuestoService(IHttpClientFactory factory) : base(factory)
    {
    }

    public Task<(ResultadoPaginado<RepuestoViewModel>? Datos, string? Error)> ObtenerPaginadoAsync(
        string? busqueda,
        string? ordenarPor,
        int pagina = 1,
        int tamanoPagina = 10,
        CancellationToken ct = default)
    {
        var parametros = new Dictionary<string, string?>
        {
            ["pagina"] = Math.Max(1, pagina).ToString(),
            ["tamanoPagina"] = Math.Clamp(tamanoPagina, 1, 50).ToString()
        };

        if (!string.IsNullOrWhiteSpace(busqueda))
            parametros["busqueda"] = busqueda.Trim();

        if (!string.IsNullOrWhiteSpace(ordenarPor))
            parametros["ordenarPor"] = ordenarPor.Trim();

        var url = QueryHelpers.AddQueryString("api/Repuestos", parametros);
        return GetPaginadoAsync<RepuestoViewModel>(url, ct);
    }

    public Task<(List<RepuestoViewModel>? Datos, string? Error)> ObtenerBajoMinimoAsync(
        CancellationToken ct = default) =>
        GetAsync<List<RepuestoViewModel>>("api/Repuestos/bajo-minimo", ct);

    public Task<(RepuestoViewModel? Datos, string? Error)> ObtenerPorCodigoAsync(
        string codigoRepuesto, CancellationToken ct = default) =>
        GetAsync<RepuestoViewModel>($"api/Repuestos/{Uri.EscapeDataString(codigoRepuesto)}", ct);

    public Task<(RepuestoViewModel? Datos, string? Error)> CrearAsync(
        CrearRepuestoViewModel modelo, CancellationToken ct = default) =>
        PostAsync<CrearRepuestoViewModel, RepuestoViewModel>("api/Repuestos", modelo, ct);

    public Task<(RepuestoViewModel? Datos, string? Error)> ActualizarAsync(
        string codigoRepuesto, ActualizarRepuestoViewModel modelo, CancellationToken ct = default)
    {
        var cuerpo = new
        {
            modelo.Nombre,
            modelo.Stock,
            modelo.StockMinimo,
            modelo.CostoUnitario,
            modelo.IdEstado,
            modelo.RowVersion
        };

        var ifMatch = string.IsNullOrWhiteSpace(modelo.RowVersion)
            ? null
            : $"\"{modelo.RowVersion}\"";

        return PutAsync<object, RepuestoViewModel>(
            $"api/Repuestos/{Uri.EscapeDataString(codigoRepuesto)}", cuerpo, ifMatch, ct);
    }

    public Task<(bool Datos, string? Error)> EliminarAsync(
        string codigoRepuesto, CancellationToken ct = default) =>
        DeleteAsync($"api/Repuestos/{Uri.EscapeDataString(codigoRepuesto)}", ct);
}
