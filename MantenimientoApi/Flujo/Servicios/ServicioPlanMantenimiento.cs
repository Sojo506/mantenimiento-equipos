using Abstracciones.Configuracion;
using Abstracciones.DTOs.Ordenes;
using Abstracciones.DTOs.Planes;
using Abstracciones.Entidades;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Microsoft.Extensions.Options;

namespace Flujo.Servicios;

public class ServicioPlanMantenimiento : IServicioPlanMantenimiento
{
    private const int EstadoActivo = 1;
    private const int EstadoInactivo = 2;
    private const int EquipoDadoDeBaja = 6;
    private const int TipoOrdenPreventiva = 1;

    private readonly IRepositorioPlanMantenimiento _repositorioPlan;
    private readonly IRepositorioEquipo _repositorioEquipo;
    private readonly IServicioOrdenDeTrabajo _servicioOrden;
    private readonly ReglasNegocioOptions _options;

    public ServicioPlanMantenimiento(
        IRepositorioPlanMantenimiento repositorioPlan,
        IRepositorioEquipo repositorioEquipo,
        IServicioOrdenDeTrabajo servicioOrden,
        IOptions<ReglasNegocioOptions> options)
    {
        _repositorioPlan = repositorioPlan;
        _repositorioEquipo = repositorioEquipo;
        _servicioOrden = servicioOrden;
        _options = options.Value;
    }

    public async Task<IEnumerable<PlanMantenimientoDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        var planes = await _repositorioPlan.ObtenerTodosAsync(cancellationToken);
        DateTime ahora = DateTime.UtcNow;
        return planes.Select(p => MapearADto(p, ahora));
    }

    public async Task<PlanMantenimientoDto> ObtenerPorIdAsync(int idPlan, CancellationToken cancellationToken = default)
    {
        var plan = await ObtenerPlanExistenteAsync(idPlan, cancellationToken);
        return MapearADto(plan, DateTime.UtcNow);
    }

    public async Task<PlanMantenimientoDto> CrearAsync(CrearPlanMantenimientoDto dto, CancellationToken cancellationToken = default)
    {
        string codigoEquipo = dto.CodigoEquipo.Trim();

        var equipo = await _repositorioEquipo.ObtenerPorCodigoAsync(codigoEquipo, cancellationToken);
        if (equipo == null)
        {
            throw new NoEncontradoException("Equipo", codigoEquipo);
        }

        if (equipo.IdEstado == EquipoDadoDeBaja)
        {
            throw new ReglaNegocioException($"El equipo '{codigoEquipo}' está 'Dado de Baja' y no admite planes de mantenimiento (RN-04).");
        }

        var existente = await _repositorioPlan.ObtenerPorEquipoAsync(codigoEquipo, cancellationToken);
        if (existente != null)
        {
            throw new ConflictoException($"El equipo '{codigoEquipo}' ya tiene un plan de mantenimiento (#{existente.IdPlanMantenimiento}). Edite ese plan en lugar de crear otro.");
        }

        var plan = new PlanDeMantenimiento
        {
            CodigoEquipo = codigoEquipo,
            Frecuencia = dto.Frecuencia,
            ProximaFecha = dto.ProximaFecha!.Value,
            IdEstado = EstadoActivo
        };

        await _repositorioPlan.AgregarAsync(plan, cancellationToken);
        await _repositorioPlan.GuardarCambiosAsync(cancellationToken);

        return await ObtenerPorIdAsync(plan.IdPlanMantenimiento, cancellationToken);
    }

    public async Task<PlanMantenimientoDto> ActualizarAsync(int idPlan, ActualizarPlanMantenimientoDto dto, CancellationToken cancellationToken = default)
    {
        var plan = await ObtenerPlanExistenteAsync(idPlan, cancellationToken);

        if (dto.IdEstado == EstadoActivo && plan.Equipo?.IdEstado == EquipoDadoDeBaja)
        {
            throw new ReglaNegocioException($"No se puede activar el plan: el equipo '{plan.CodigoEquipo}' está 'Dado de Baja' (RN-04).");
        }

        plan.Frecuencia = dto.Frecuencia;
        plan.ProximaFecha = dto.ProximaFecha!.Value;
        plan.IdEstado = dto.IdEstado;

        await _repositorioPlan.ActualizarAsync(plan, cancellationToken);
        await _repositorioPlan.GuardarCambiosAsync(cancellationToken);

        return await ObtenerPorIdAsync(idPlan, cancellationToken);
    }

    public async Task EliminarAsync(int idPlan, CancellationToken cancellationToken = default)
    {
        var plan = await ObtenerPlanExistenteAsync(idPlan, cancellationToken);

        await _repositorioPlan.EliminarAsync(plan, cancellationToken);
        await _repositorioPlan.GuardarCambiosAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<int>> GenerarOrdenesPreventivasAsync(CancellationToken cancellationToken = default)
    {
        DateTime ahora = DateTime.UtcNow;
        var pendientes = await _repositorioPlan.ObtenerPlanesPendientesAsync(ahora, cancellationToken);
        var generadas = new List<int>();

        foreach (var plan in pendientes)
        {
            if (plan.Equipo?.IdEstado == EquipoDadoDeBaja)
            {
                plan.IdEstado = EstadoInactivo;
                await _repositorioPlan.ActualizarAsync(plan, cancellationToken);
                await _repositorioPlan.GuardarCambiosAsync(cancellationToken);
                continue;
            }

            DateTime fechaProgramada = plan.ProximaFecha;

            plan.ProximaFecha = CalcularSiguienteFecha(plan.ProximaFecha, plan.Frecuencia, ahora);
            await _repositorioPlan.ActualizarAsync(plan, cancellationToken);

            var orden = await _servicioOrden.CrearAsync(new CrearOrdenDto
            {
                CodigoEquipo = plan.CodigoEquipo,
                IdentificacionSolicitante = _options.IdentificacionUsuarioSistema,
                IdTipoOrden = TipoOrdenPreventiva,
                Prioridad = _options.PrioridadOrdenPreventiva,
                Descripcion = $"Mantenimiento preventivo programado (RN-06) del plan #{plan.IdPlanMantenimiento}: " +
                              $"frecuencia cada {plan.Frecuencia} días, fecha programada {fechaProgramada:yyyy-MM-dd}."
            }, cancellationToken);

            await _repositorioPlan.GuardarCambiosAsync(cancellationToken);
            generadas.Add(orden.Numero);
        }

        return generadas;
    }

    public static DateTime CalcularSiguienteFecha(DateTime fechaProgramada, int frecuenciaDias, DateTime ahora)
    {
        if (frecuenciaDias <= 0)
        {
            throw new ReglaNegocioException("La frecuencia del plan debe ser mayor a 0 días.");
        }

        DateTime siguiente = fechaProgramada;
        while (siguiente <= ahora)
        {
            siguiente = siguiente.AddDays(frecuenciaDias);
        }

        return siguiente;
    }

    private async Task<PlanDeMantenimiento> ObtenerPlanExistenteAsync(int idPlan, CancellationToken cancellationToken)
    {
        var plan = await _repositorioPlan.ObtenerPorIdAsync(idPlan, cancellationToken);
        if (plan == null)
        {
            throw new NoEncontradoException("PlanDeMantenimiento", idPlan);
        }

        return plan;
    }

    private static PlanMantenimientoDto MapearADto(PlanDeMantenimiento p, DateTime ahora)
    {
        return new PlanMantenimientoDto
        {
            IdPlanMantenimiento = p.IdPlanMantenimiento,
            CodigoEquipo = p.CodigoEquipo,
            EquipoNombre = p.Equipo?.Nombre ?? string.Empty,
            Frecuencia = p.Frecuencia,
            ProximaFecha = p.ProximaFecha,
            IdEstado = p.IdEstado,
            EstadoNombre = p.Estado?.Nombre ?? (p.IdEstado == EstadoActivo ? "Activo" : "Inactivo"),
            Vencido = p.IdEstado == EstadoActivo && p.ProximaFecha <= ahora
        };
    }
}
