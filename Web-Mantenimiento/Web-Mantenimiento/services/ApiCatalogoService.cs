using Microsoft.AspNetCore.Mvc.Rendering;
using Web_Mantenimiento.Models.Catalogos;
using Web_Mantenimiento.Services.Base;

namespace Web_Mantenimiento.Services;

public class ApiCatalogoService : ApiClientBase
{
    public ApiCatalogoService(IHttpClientFactory factory) : base(factory)
    {
    }

    public Task<List<SelectListItem>> ObtenerEstadosAsync(CancellationToken ct = default) =>
        ObtenerSelectListAsync<EstadoDto>("api/Catalogos/estados", x => x.IdEstado, x => x.Nombre, ct);

    public Task<List<SelectListItem>> ObtenerTiposUsuarioAsync(CancellationToken ct = default) =>
        ObtenerSelectListAsync<TipoUsuarioDto>("api/Catalogos/tipos-usuario", x => x.IdTipoUsuario, x => x.Nombre, ct);

    public Task<List<SelectListItem>> ObtenerTiposEquipoAsync(CancellationToken ct = default) =>
        ObtenerSelectListAsync<TipoEquipoDto>("api/Catalogos/tipos-equipo", x => x.IdTipoEquipo, x => x.Nombre, ct);

    public Task<List<SelectListItem>> ObtenerMarcasAsync(CancellationToken ct = default) =>
        ObtenerSelectListAsync<MarcaDto>("api/Catalogos/marcas", x => x.IdMarca, x => x.Nombre, ct);

    public Task<List<SelectListItem>> ObtenerModelosAsync(CancellationToken ct = default) =>
        ObtenerSelectListAsync<ModeloDto>("api/Catalogos/modelos", x => x.IdModelo, x => x.Nombre, ct);

    public Task<List<SelectListItem>> ObtenerUbicacionesAsync(CancellationToken ct = default) =>
        ObtenerSelectListAsync<UbicacionDto>("api/Catalogos/ubicaciones", x => x.IdUbicacion, x => x.Nombre, ct);

    public Task<List<SelectListItem>> ObtenerEspecialidadesAsync(CancellationToken ct = default) =>
        ObtenerSelectListAsync<EspecialidadDto>("api/Catalogos/especialidades", x => x.IdEspecialidad, x => x.Nombre, ct);

    public Task<List<SelectListItem>> ObtenerTiposOrdenAsync(CancellationToken ct = default) =>
        ObtenerSelectListAsync<TipoOrdenDto>("api/Catalogos/tipos-orden", x => x.IdTipoOrden, x => x.Nombre, ct);

    public Task<List<SelectListItem>> ObtenerTiposGravedadAsync(CancellationToken ct = default) =>
        ObtenerSelectListAsync<TipoGravedadDto>("api/Catalogos/tipos-gravedad", x => x.IdTipoGravedad, x => x.Nombre, ct);

    private async Task<List<SelectListItem>> ObtenerSelectListAsync<TDto>(
        string url,
        Func<TDto, int> idSelector,
        Func<TDto, string> nombreSelector,
        CancellationToken ct)
    {
        var (datos, _) = await GetAsync<List<TDto>>(url, ct);
        if (datos == null)
            return new List<SelectListItem>();

        return datos
            .Select(d => new SelectListItem
            {
                Value = idSelector(d).ToString(),
                Text = nombreSelector(d)
            })
            .ToList();
    }
}
