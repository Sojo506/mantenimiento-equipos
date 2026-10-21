using Abstracciones.DTOs.Equipos;
using Abstracciones.Entidades;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;

namespace Flujo.Servicios;

/// <summary>
/// Implementación de la lógica de negocio para la administración de equipos.
/// </summary>
public class ServicioEquipo : IServicioEquipo
{
    private readonly IRepositorioEquipo _repositorioEquipo;
    private readonly IRepositorioOrdenDeTrabajo _repositorioOrden;

    public ServicioEquipo(IRepositorioEquipo repositorioEquipo, IRepositorioOrdenDeTrabajo repositorioOrden)
    {
        _repositorioEquipo = repositorioEquipo;
        _repositorioOrden = repositorioOrden;
    }

    public async Task<IEnumerable<EquipoDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        var equipos = await _repositorioEquipo.ObtenerTodosAsync(cancellationToken);
        return equipos.Select(MapearADto);
    }

    public async Task<EquipoDto> ObtenerPorCodigoAsync(string codigoEquipo, CancellationToken cancellationToken = default)
    {
        var equipo = await _repositorioEquipo.ObtenerPorCodigoAsync(codigoEquipo, cancellationToken);
        if (equipo == null)
        {
            throw new NoEncontradoException("Equipo", codigoEquipo);
        }

        return MapearADto(equipo);
    }

    public async Task<EquipoDto> CrearAsync(CrearEquipoDto dto, CancellationToken cancellationToken = default)
    {
        if (await _repositorioEquipo.ExisteCodigoAsync(dto.CodigoEquipo, cancellationToken))
        {
            throw new ReglaNegocioException($"Ya existe un equipo registrado con el código '{dto.CodigoEquipo}'.");
        }

        var nuevoEquipo = new Equipo
        {
            CodigoEquipo = dto.CodigoEquipo.Trim().ToUpper(),
            Nombre = dto.Nombre.Trim(),
            IdTipoEquipo = dto.IdTipoEquipo,
            IdMarca = dto.IdMarca,
            IdModelo = dto.IdModelo,
            IdUbicacion = dto.IdUbicacion,
            FechaAdquisicion = dto.FechaAdquisicion,
            IdEstado = dto.IdEstado
        };

        await _repositorioEquipo.AgregarAsync(nuevoEquipo, cancellationToken);
        await _repositorioEquipo.GuardarCambiosAsync(cancellationToken);

        return await ObtenerPorCodigoAsync(nuevoEquipo.CodigoEquipo, cancellationToken);
    }

    public async Task<EquipoDto> ActualizarAsync(string codigoEquipo, ActualizarEquipoDto dto, CancellationToken cancellationToken = default)
    {
        var equipo = await _repositorioEquipo.ObtenerPorCodigoAsync(codigoEquipo, cancellationToken);
        if (equipo == null)
        {
            throw new NoEncontradoException("Equipo", codigoEquipo);
        }

        equipo.Nombre = dto.Nombre.Trim();
        equipo.IdTipoEquipo = dto.IdTipoEquipo;
        equipo.IdMarca = dto.IdMarca;
        equipo.IdModelo = dto.IdModelo;
        equipo.IdUbicacion = dto.IdUbicacion;
        equipo.IdEstado = dto.IdEstado;

        await _repositorioEquipo.ActualizarAsync(equipo, cancellationToken);
        await _repositorioEquipo.GuardarCambiosAsync(cancellationToken);

        return await ObtenerPorCodigoAsync(codigoEquipo, cancellationToken);
    }

    public async Task EliminarAsync(string codigoEquipo, CancellationToken cancellationToken = default)
    {
        var equipo = await _repositorioEquipo.ObtenerPorCodigoAsync(codigoEquipo, cancellationToken);
        if (equipo == null)
        {
            throw new NoEncontradoException("Equipo", codigoEquipo);
        }

        var ordenes = await _repositorioOrden.ObtenerPorEquipoAsync(codigoEquipo, cancellationToken);
        if (ordenes.Any())
        {
            throw new ReglaNegocioException($"No se puede eliminar el equipo '{codigoEquipo}' porque tiene órdenes de trabajo registradas.");
        }

        await _repositorioEquipo.EliminarAsync(codigoEquipo, cancellationToken);
        await _repositorioEquipo.GuardarCambiosAsync(cancellationToken);
    }

    private static EquipoDto MapearADto(Equipo e)
    {
        return new EquipoDto
        {
            CodigoEquipo = e.CodigoEquipo,
            Nombre = e.Nombre,
            IdTipoEquipo = e.IdTipoEquipo,
            TipoEquipoNombre = e.TipoEquipo?.Nombre ?? string.Empty,
            IdMarca = e.IdMarca,
            MarcaNombre = e.Marca?.Nombre ?? string.Empty,
            IdModelo = e.IdModelo,
            ModeloNombre = e.Modelo?.Nombre ?? string.Empty,
            IdUbicacion = e.IdUbicacion,
            UbicacionNombre = e.Ubicacion?.Nombre ?? string.Empty,
            FechaAdquisicion = e.FechaAdquisicion,
            IdEstado = e.IdEstado,
            EstadoNombre = e.Estado?.Nombre ?? string.Empty
        };
    }
}
