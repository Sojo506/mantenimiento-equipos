using Abstracciones.Configuracion;
using Abstracciones.DTOs.Ordenes;
using Abstracciones.DTOs.Planes;
using Abstracciones.Entidades;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Flujo.Servicios;
using Microsoft.Extensions.Options;
using Moq;

namespace Flujo.Tests.Servicios;

public class ServicioPlanMantenimientoTests
{
    private const string UsuarioSistema = "USR-SYS";

    private readonly Mock<IRepositorioPlanMantenimiento> _repoPlan = new();
    private readonly Mock<IRepositorioEquipo> _repoEquipo = new();
    private readonly Mock<IServicioOrdenDeTrabajo> _servicioOrden = new();
    private readonly List<CrearOrdenDto> _ordenesSolicitadas = new();
    private readonly ServicioPlanMantenimiento _servicio;

    public ServicioPlanMantenimientoTests()
    {
        int siguienteNumero = 500;
        _servicioOrden
            .Setup(s => s.CrearAsync(It.IsAny<CrearOrdenDto>(), It.IsAny<CancellationToken>()))
            .Callback<CrearOrdenDto, CancellationToken>((dto, _) => _ordenesSolicitadas.Add(dto))
            .ReturnsAsync(() => new OrdenDeTrabajoDto { Numero = ++siguienteNumero });

        var options = Options.Create(new ReglasNegocioOptions
        {
            IdentificacionUsuarioSistema = UsuarioSistema,
            PrioridadOrdenPreventiva = "Media"
        });

        _servicio = new ServicioPlanMantenimiento(_repoPlan.Object, _repoEquipo.Object, _servicioOrden.Object, options);
    }

    [Fact]
    public async Task GenerarOrdenesPreventivasAsync_PlanVencido_CreaOrdenPreventivaYReprogramaFecha_RN06()
    {
        var plan = new PlanDeMantenimiento
        {
            IdPlanMantenimiento = 1,
            CodigoEquipo = "EQ-001",
            Frecuencia = 30,
            ProximaFecha = DateTime.UtcNow.Date.AddDays(-1),
            IdEstado = 1,
            Equipo = new Equipo { CodigoEquipo = "EQ-001", IdEstado = 3 }
        };
        DateTime fechaOriginal = plan.ProximaFecha;
        _repoPlan.Setup(r => r.ObtenerPlanesPendientesAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { plan });

        var generadas = await _servicio.GenerarOrdenesPreventivasAsync();

        Assert.Equal(new[] { 501 }, generadas);
        var orden = Assert.Single(_ordenesSolicitadas);
        Assert.Equal("EQ-001", orden.CodigoEquipo);
        Assert.Equal(1, orden.IdTipoOrden);
        Assert.Equal("Media", orden.Prioridad);
        Assert.Equal(UsuarioSistema, orden.IdentificacionSolicitante);
        Assert.Contains("RN-06", orden.Descripcion);

        Assert.Equal(fechaOriginal.AddDays(30), plan.ProximaFecha);
        _repoPlan.Verify(r => r.ActualizarAsync(plan, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GenerarOrdenesPreventivasAsync_VariosPeriodosVencidos_GeneraUnaSolaOrdenYFechaFutura_RN06()
    {
        var plan = new PlanDeMantenimiento
        {
            IdPlanMantenimiento = 2,
            CodigoEquipo = "EQ-002",
            Frecuencia = 7,
            ProximaFecha = DateTime.UtcNow.AddDays(-30),
            IdEstado = 1,
            Equipo = new Equipo { CodigoEquipo = "EQ-002", IdEstado = 3 }
        };
        _repoPlan.Setup(r => r.ObtenerPlanesPendientesAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { plan });

        var generadas = await _servicio.GenerarOrdenesPreventivasAsync();

        Assert.Single(generadas);
        Assert.True(plan.ProximaFecha > DateTime.UtcNow);
        Assert.True(plan.ProximaFecha <= DateTime.UtcNow.AddDays(7));
    }

    [Fact]
    public async Task GenerarOrdenesPreventivasAsync_EquipoDadoDeBaja_DesactivaPlanSinCrearOrden_RN06()
    {
        var plan = new PlanDeMantenimiento
        {
            IdPlanMantenimiento = 3,
            CodigoEquipo = "EQ-008",
            Frecuencia = 30,
            ProximaFecha = DateTime.UtcNow.AddDays(-2),
            IdEstado = 1,
            Equipo = new Equipo { CodigoEquipo = "EQ-008", IdEstado = 6 }
        };
        _repoPlan.Setup(r => r.ObtenerPlanesPendientesAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { plan });

        var generadas = await _servicio.GenerarOrdenesPreventivasAsync();

        Assert.Empty(generadas);
        Assert.Empty(_ordenesSolicitadas);
        Assert.Equal(2, plan.IdEstado);
    }

    [Fact]
    public async Task CrearAsync_EquipoConPlanExistente_LanzaConflictoException()
    {
        _repoEquipo.Setup(r => r.ObtenerPorCodigoAsync("EQ-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Equipo { CodigoEquipo = "EQ-001", IdEstado = 3 });
        _repoPlan.Setup(r => r.ObtenerPorEquipoAsync("EQ-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PlanDeMantenimiento { IdPlanMantenimiento = 1, CodigoEquipo = "EQ-001" });

        var dto = new CrearPlanMantenimientoDto { CodigoEquipo = "EQ-001", Frecuencia = 15, ProximaFecha = DateTime.Today.AddDays(5) };

        await Assert.ThrowsAsync<ConflictoException>(() => _servicio.CrearAsync(dto));
        _repoPlan.Verify(r => r.AgregarAsync(It.IsAny<PlanDeMantenimiento>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CrearAsync_EquipoDadoDeBaja_LanzaReglaNegocioException()
    {
        _repoEquipo.Setup(r => r.ObtenerPorCodigoAsync("EQ-008", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Equipo { CodigoEquipo = "EQ-008", IdEstado = 6 });

        var dto = new CrearPlanMantenimientoDto { CodigoEquipo = "EQ-008", Frecuencia = 15, ProximaFecha = DateTime.Today };

        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.CrearAsync(dto));
    }

    [Theory]
    [InlineData(-1, 30, 29)]
    [InlineData(-10, 7, 4)]
    [InlineData(5, 30, 5)]
    public void CalcularSiguienteFecha_DevuelvePrimeraFechaPosteriorAHoy(int desfaseDias, int frecuencia, int esperadoDias)
    {
        DateTime ahora = new(2026, 11, 23, 12, 0, 0);
        DateTime programada = ahora.Date.AddDays(desfaseDias);

        DateTime siguiente = ServicioPlanMantenimiento.CalcularSiguienteFecha(programada, frecuencia, ahora);

        Assert.Equal(ahora.Date.AddDays(esperadoDias), siguiente);
    }
}
