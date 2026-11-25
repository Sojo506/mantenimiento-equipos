using Microsoft.AspNetCore.WebUtilities;
using Web_Mantenimiento.Models.Comun;
using Web_Mantenimiento.Models.Planes;
using Web_Mantenimiento.Services.Base;

namespace Web_Mantenimiento.Services;

public class ApiPlanService : ApiClientBase
{
    public ApiPlanService(IHttpClientFactory factory) : base(factory)
    {
    }

    public Task<(ResultadoPaginado<PlanViewModel>? Datos, string? Error)> ObtenerPaginadoAsync(
        string? busqueda, string? ordenarPor, int? idEstado, bool soloVencidos,
        int pagina = 1, int tamanoPagina = 10, CancellationToken ct = default)
    {
        var p = new Dictionary<string, string?>
        {
            ["pagina"] = Math.Max(1, pagina).ToString(),
            ["tamanoPagina"] = Math.Clamp(tamanoPagina, 1, 50).ToString()
        };

        if (!string.IsNullOrWhiteSpace(busqueda)) p["busqueda"] = busqueda.Trim();
        if (!string.IsNullOrWhiteSpace(ordenarPor)) p["ordenarPor"] = ordenarPor.Trim();
        if (idEstado.HasValue) p["idEstado"] = idEstado.Value.ToString();
        if (soloVencidos) p["soloVencidos"] = "true";

        return GetPaginadoAsync<PlanViewModel>(QueryHelpers.AddQueryString("api/planes", p), ct);
    }

    public Task<(PlanViewModel? Datos, string? Error)> ObtenerPorIdAsync(int idPlan, CancellationToken ct = default) =>
        GetAsync<PlanViewModel>($"api/planes/{idPlan}", ct);

    public Task<(PlanViewModel? Datos, string? Error)> CrearAsync(CrearPlanViewModel modelo, CancellationToken ct = default) =>
        PostAsync<object, PlanViewModel>("api/planes", new
        {
            modelo.CodigoEquipo,
            modelo.Frecuencia,
            modelo.ProximaFecha
        }, ct);

    public Task<(PlanViewModel? Datos, string? Error)> ActualizarAsync(EditarPlanViewModel modelo, CancellationToken ct = default) =>
        PutAsync<object, PlanViewModel>($"api/planes/{modelo.IdPlanMantenimiento}", new
        {
            modelo.Frecuencia,
            modelo.ProximaFecha,
            modelo.IdEstado
        }, ct: ct);

    public Task<(bool Datos, string? Error)> EliminarAsync(int idPlan, CancellationToken ct = default) =>
        DeleteAsync($"api/planes/{idPlan}", ct);

    public Task<(ResultadoProcesoViewModel? Datos, string? Error)> EjecutarReglasAutomaticasAsync(CancellationToken ct = default) =>
        PostAsync<object, ResultadoProcesoViewModel>("api/procesos/reglas-automaticas", new { }, ct);
}
