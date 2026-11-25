using Microsoft.AspNetCore.WebUtilities;
using Web_Mantenimiento.Models.Comun;
using Web_Mantenimiento.Models.Equipos;
using Web_Mantenimiento.Services.Base;

namespace Web_Mantenimiento.Services;

public class ApiEquipoService : ApiClientBase
{
    public ApiEquipoService(IHttpClientFactory factory) : base(factory)
    {
    }

    public Task<(ResultadoPaginado<EquipoViewModel>? Datos, string? Error)> ObtenerPaginadoAsync(
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

        var url = QueryHelpers.AddQueryString("api/Equipos", parametros);
        return GetPaginadoAsync<EquipoViewModel>(url, ct);
    }

    public Task<(EquipoViewModel? Datos, string? Error)> ObtenerPorCodigoAsync(
        string codigoEquipo, CancellationToken ct = default) =>
        GetAsync<EquipoViewModel>($"api/Equipos/{Uri.EscapeDataString(codigoEquipo)}", ct);

    public Task<(EquipoViewModel? Datos, string? Error)> CrearAsync(
        CrearEquipoViewModel modelo, CancellationToken ct = default) =>
        PostAsync<CrearEquipoViewModel, EquipoViewModel>("api/Equipos", modelo, ct);

    public Task<(EquipoViewModel? Datos, string? Error)> ActualizarAsync(
        string codigoEquipo, ActualizarEquipoViewModel modelo, CancellationToken ct = default)
    {
        var cuerpo = new
        {
            modelo.Nombre,
            modelo.IdTipoEquipo,
            modelo.IdMarca,
            modelo.IdModelo,
            modelo.IdUbicacion,
            modelo.IdEstado
        };

        return PutAsync<object, EquipoViewModel>(
            $"api/Equipos/{Uri.EscapeDataString(codigoEquipo)}", cuerpo, ct: ct);
    }

    public Task<(bool Datos, string? Error)> EliminarAsync(
        string codigoEquipo, CancellationToken ct = default) =>
        DeleteAsync($"api/Equipos/{Uri.EscapeDataString(codigoEquipo)}", ct);

    public Task<(PrediccionFallaEquipoViewModel? Datos, string? Error)> ObtenerPrediccionAsync(
        string codigoEquipo, CancellationToken ct = default) =>
        GetAsync<PrediccionFallaEquipoViewModel>($"api/Equipos/{Uri.EscapeDataString(codigoEquipo)}/prediccion", ct);
}
