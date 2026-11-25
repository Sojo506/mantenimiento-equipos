using Abstracciones.Configuracion;
using Abstracciones.DTOs.Fallas;
using Abstracciones.Entidades;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Microsoft.Extensions.Options;

namespace Flujo.Servicios;

/// <summary>
/// Implementación de la lógica de negocio para el registro de fallas de equipos
/// y la transición automática a estado EnRiesgo al superar el umbral de fallas recurrentes (RN-03).
/// </summary>
public class ServicioFalla : IServicioFalla
{
    private const int MesesVentanaRiesgo = 6;
    private readonly IRepositorioFallaRegistrada _repositorioFalla;
    private readonly IRepositorioEquipo _repositorioEquipo;
    private readonly IRepositorioOrdenDeTrabajo _repositorioOrden;
    private readonly IRepositorioBitacoraOrden _repositorioBitacora;
    private readonly ReglasNegocioOptions _options;

    public ServicioFalla(
        IRepositorioFallaRegistrada repositorioFalla,
        IRepositorioEquipo repositorioEquipo,
        IRepositorioOrdenDeTrabajo repositorioOrden,
        IRepositorioBitacoraOrden repositorioBitacora,
        IOptions<ReglasNegocioOptions> options)
    {
        _repositorioFalla = repositorioFalla;
        _repositorioEquipo = repositorioEquipo;
        _repositorioOrden = repositorioOrden;
        _repositorioBitacora = repositorioBitacora;
        _options = options.Value;
    }

    public async Task<IEnumerable<FallaRegistradaDto>> ObtenerTodasAsync(CancellationToken cancellationToken = default)
    {
        var fallas = await _repositorioFalla.ObtenerTodasAsync(cancellationToken);
        return fallas.Select(f => MapearADto(f, false));
    }

    public async Task<IEnumerable<FallaRegistradaDto>> ObtenerPorEquipoAsync(string codigoEquipo, CancellationToken cancellationToken = default)
    {
        string codigo = codigoEquipo.Trim().ToUpperInvariant();
        var equipo = await _repositorioEquipo.ObtenerPorCodigoAsync(codigo, cancellationToken);
        if (equipo == null)
        {
            throw new NoEncontradoException("Equipo", codigo);
        }

        var fallas = await _repositorioFalla.ObtenerPorEquipoAsync(codigo, cancellationToken);
        return fallas.Select(f => MapearADto(f, false));
    }

    public async Task<FallaRegistradaDto> ObtenerPorIdAsync(int idFalla, CancellationToken cancellationToken = default)
    {
        var falla = await _repositorioFalla.ObtenerPorIdAsync(idFalla, cancellationToken);
        if (falla == null)
        {
            throw new NoEncontradoException("FallaRegistrada", idFalla);
        }

        return MapearADto(falla, false);
    }

    public async Task<FallaRegistradaDto> RegistrarAsync(RegistrarFallaDto dto, CancellationToken cancellationToken = default)
    {
        string codigoEquipo = dto.CodigoEquipo.Trim().ToUpperInvariant();
        var equipo = await _repositorioEquipo.ObtenerPorCodigoAsync(codigoEquipo, cancellationToken);
        if (equipo == null)
        {
            throw new NoEncontradoException("Equipo", codigoEquipo);
        }

        if (equipo.IdEstado == 6) // 6: DadoDeBaja
        {
            throw new ReglaNegocioException($"No se pueden reportar fallas para el equipo '{codigoEquipo}' porque se encuentra 'Dado de Baja' (RN-04).");
        }

        if (dto.IdTipoGravedad < 1 || dto.IdTipoGravedad > 4)
        {
            throw new ReglaNegocioException("El nivel de gravedad de la falla debe estar entre 1 (Baja) y 4 (Critica).");
        }

        int numeroOrden = await ResolverOrdenAsociadaAsync(equipo.CodigoEquipo, dto, cancellationToken);

        var nuevaFalla = new FallaRegistrada
        {
            CodigoEquipo = equipo.CodigoEquipo,
            Fecha = DateTime.UtcNow,
            IdTipoGravedad = dto.IdTipoGravedad,
            Descripcion = dto.Descripcion.Trim(),
            NumeroOrden = numeroOrden
        };

        await _repositorioFalla.AgregarAsync(nuevaFalla, cancellationToken);
        await _repositorioFalla.GuardarCambiosAsync(cancellationToken);

        // RN-03: Si el equipo acumula >= MaxFallasParaRiesgo fallas en los últimos 6 meses, pasa automáticamente a EnRiesgo (IdEstado = 5)
        int fallasRecientes = await _repositorioFalla.ContarFallasUltimosMesesAsync(equipo.CodigoEquipo, MesesVentanaRiesgo, cancellationToken);
        bool pasoAEnRiesgo = false;

        if (fallasRecientes >= _options.MaxFallasParaRiesgo && equipo.IdEstado != 5 && equipo.IdEstado != 6)
        {
            equipo.IdEstado = 5; // 5: En Riesgo
            await _repositorioEquipo.ActualizarAsync(equipo, cancellationToken);
            await _repositorioEquipo.GuardarCambiosAsync(cancellationToken);
            pasoAEnRiesgo = true;
        }

        var fallaGuardada = await _repositorioFalla.ObtenerPorIdAsync(nuevaFalla.Id, cancellationToken);
        if (fallaGuardada != null)
        {
            return MapearADto(fallaGuardada, pasoAEnRiesgo);
        }

        return new FallaRegistradaDto
        {
            Id = nuevaFalla.Id,
            CodigoEquipo = equipo.CodigoEquipo,
            EquipoNombre = equipo.Nombre,
            EstadoEquipoActual = equipo.IdEstado,
            EstadoEquipoNombre = equipo.IdEstado == 5 ? "En Riesgo" : (equipo.Estado?.Nombre ?? "Operativo"),
            Fecha = nuevaFalla.Fecha,
            IdTipoGravedad = nuevaFalla.IdTipoGravedad,
            GravedadNombre = ObtenerNombreGravedad(nuevaFalla.IdTipoGravedad),
            Descripcion = nuevaFalla.Descripcion,
            NumeroOrden = numeroOrden,
            PasoAEnRiesgoAutomatico = pasoAEnRiesgo
        };
    }

    private async Task<int> ResolverOrdenAsociadaAsync(string codigoEquipo, RegistrarFallaDto dto, CancellationToken cancellationToken)
    {
        if (dto.NumeroOrden.HasValue && dto.NumeroOrden.Value > 0)
        {
            var ordenIndicada = await _repositorioOrden.ObtenerPorNumeroAsync(dto.NumeroOrden.Value, cancellationToken);
            if (ordenIndicada == null)
            {
                throw new NoEncontradoException("OrdenDeTrabajo", dto.NumeroOrden.Value);
            }
            return ordenIndicada.Numero;
        }

        // Buscar si el equipo ya tiene una orden abierta o en curso para asociar la falla
        var ordenesEquipo = (await _repositorioOrden.ObtenerPorEquipoAsync(codigoEquipo, cancellationToken)).ToList();
        var ordenActiva = ordenesEquipo
            .OrderByDescending(o => o.FechaCreacion)
            .FirstOrDefault(o => o.IdEstado != 11);

        if (ordenActiva != null)
        {
            return ordenActiva.Numero;
        }

        // Si no tiene ninguna orden activa, creamos una orden correctiva para la falla reportada
        string prioridad = ObtenerNombreGravedad(dto.IdTipoGravedad);
        int horasPlazo = prioridad switch
        {
            "Critica" => _options.PlazoHorasCritica,
            "Alta" => _options.PlazoHorasAlta,
            "Media" => _options.PlazoHorasMedia,
            _ => _options.PlazoHorasBaja
        };

        DateTime ahora = DateTime.UtcNow;
        string solicitante = string.IsNullOrWhiteSpace(dto.IdentificacionSolicitante)
            ? "USR-005"
            : dto.IdentificacionSolicitante.Trim();

        var nuevaOrden = new OrdenDeTrabajo
        {
            CodigoEquipo = codigoEquipo,
            IdentificacionSolicitante = solicitante,
            IdentificacionTecnico = null,
            IdTipoOrden = 2, // 2: Correctiva
            Prioridad = prioridad,
            FechaCreacion = ahora,
            FechaLimite = ahora.AddHours(horasPlazo),
            TiempoInvertido = 0,
            Descripcion = $"[Reporte de Falla] {dto.Descripcion.Trim()}",
            SinRepuestos = false,
            IdEstado = 7 // 7: Abierta
        };

        await _repositorioOrden.AgregarAsync(nuevaOrden, cancellationToken);
        await _repositorioOrden.GuardarCambiosAsync(cancellationToken);

        await _repositorioBitacora.RegistrarAsync(new BitacoraOrden
        {
            Numero = nuevaOrden.Numero,
            Identificacion = solicitante,
            Fecha = ahora,
            EstadoAnterior = "Ninguno",
            EstadoNuevo = "Abierta",
            Comentario = "Apertura automática de orden correctiva a partir de reporte de falla."
        }, cancellationToken);
        await _repositorioBitacora.GuardarCambiosAsync(cancellationToken);

        return nuevaOrden.Numero;
    }

    private static FallaRegistradaDto MapearADto(FallaRegistrada f, bool pasoAEnRiesgo)
    {
        return new FallaRegistradaDto
        {
            Id = f.Id,
            CodigoEquipo = f.CodigoEquipo,
            EquipoNombre = f.Equipo?.Nombre ?? string.Empty,
            EstadoEquipoActual = f.Equipo?.IdEstado ?? 0,
            EstadoEquipoNombre = f.Equipo?.Estado?.Nombre ?? string.Empty,
            Fecha = f.Fecha,
            IdTipoGravedad = f.IdTipoGravedad,
            GravedadNombre = f.TipoGravedad?.Nombre ?? ObtenerNombreGravedad(f.IdTipoGravedad),
            Descripcion = f.Descripcion,
            NumeroOrden = f.NumeroOrden,
            PasoAEnRiesgoAutomatico = pasoAEnRiesgo
        };
    }

    private static string ObtenerNombreGravedad(int idTipoGravedad) => idTipoGravedad switch
    {
        4 => "Critica",
        3 => "Alta",
        2 => "Media",
        _ => "Baja"
    };
}
