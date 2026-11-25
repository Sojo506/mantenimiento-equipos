using Web_Mantenimiento.Models.Fallas;
using Web_Mantenimiento.Services.Base;

namespace Web_Mantenimiento.Services;

public class ApiFallaService : ApiClientBase
{
    public ApiFallaService(IHttpClientFactory factory) : base(factory)
    {
    }

    public Task<(List<FallaViewModel>? Datos, string? Error)> ObtenerTodasAsync(
        string? codigoEquipo = null, CancellationToken ct = default)
    {
        var url = string.IsNullOrWhiteSpace(codigoEquipo)
            ? "api/Fallas"
            : $"api/Fallas/equipo/{Uri.EscapeDataString(codigoEquipo.Trim())}";
        return GetAsync<List<FallaViewModel>>(url, ct);
    }

    public Task<(FallaViewModel? Datos, string? Error)> RegistrarAsync(
        RegistrarFallaViewModel modelo, string identificacionSolicitante, CancellationToken ct = default)
    {
        var cuerpo = new
        {
            modelo.CodigoEquipo,
            modelo.IdTipoGravedad,
            modelo.Descripcion,
            IdentificacionSolicitante = identificacionSolicitante
        };

        return PostAsync<object, FallaViewModel>("api/Fallas", cuerpo, ct);
    }
}
