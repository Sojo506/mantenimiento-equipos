using Microsoft.AspNetCore.WebUtilities;
using Web_Mantenimiento.Models.Reportes;
using Web_Mantenimiento.Services.Base;

namespace Web_Mantenimiento.Services;

public class ApiReporteService : ApiClientBase
{
    public ApiReporteService(IHttpClientFactory factory) : base(factory)
    {
    }

    public Task<(List<ReporteConsumoRepuestoViewModel>? Datos, string? Error)> ObtenerConsumoRepuestosAsync(
        DateTime fechaInicio,
        DateTime fechaFin,
        CancellationToken ct = default)
    {
        var p = new Dictionary<string, string?>
        {
            ["fechaInicio"] = fechaInicio.ToString("yyyy-MM-dd"),
            ["fechaFin"] = fechaFin.ToString("yyyy-MM-dd")
        };

        var url = QueryHelpers.AddQueryString("api/Reportes/consumo-repuestos", p);
        return GetAsync<List<ReporteConsumoRepuestoViewModel>>(url, ct);
    }

    public async Task<(byte[]? Bytes, string? NombreArchivo, string? Error)> DescargarCsvAsync(
        string tipoReporte,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        CancellationToken ct = default)
    {
        string ruta = tipoReporte switch
        {
            "consumo-repuestos" => "api/Reportes/consumo-repuestos/csv",
            "ordenes" => "api/Reportes/ordenes/csv",
            "equipos-criticos" => "api/Reportes/equipos-criticos/csv",
            "productividad-tecnicos" => "api/Reportes/productividad-tecnicos/csv",
            _ => "api/Reportes/consumo-repuestos/csv"
        };

        var p = new Dictionary<string, string?>();
        if (fechaInicio.HasValue) p["fechaInicio"] = fechaInicio.Value.ToString("yyyy-MM-dd");
        if (fechaFin.HasValue) p["fechaFin"] = fechaFin.Value.ToString("yyyy-MM-dd");

        var url = p.Count > 0 ? QueryHelpers.AddQueryString(ruta, p) : ruta;

        try
        {
            var resp = await HttpClient.GetAsync(url, ct);
            if (!resp.IsSuccessStatusCode)
            {
                return (null, null, await ObtenerErrorAsync(resp, ct));
            }

            var bytes = await resp.Content.ReadAsByteArrayAsync(ct);
            var nombreArchivo = resp.Content.Headers.ContentDisposition?.FileName?.Trim('"')
                ?? $"reporte_{tipoReporte}_{DateTime.UtcNow:yyyyMMdd}.csv";

            return (bytes, nombreArchivo, null);
        }
        catch (HttpRequestException)
        {
            return (null, null, "No fue posible conectar con la API para generar el reporte CSV.");
        }
    }
}
