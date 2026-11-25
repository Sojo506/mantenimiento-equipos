using Abstracciones.Configuracion;
using Abstracciones.DTOs.Equipos;
using Abstracciones.DTOs.Fallas;
using Abstracciones.Entidades;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces.DA;
using Flujo.Servicios;
using Microsoft.Extensions.Options;
using Moq;

namespace Flujo.Tests.Servicios;

/// <summary>
/// Pruebas unitarias para ServicioFalla (RN-03: paso automático a EnRiesgo) y ServicioPrediccionFalla (ML.NET).
/// </summary>
public class ServicioFallaYPrediccionTests
{
    private readonly Mock<IRepositorioFallaRegistrada> _mockRepoFalla = new();
    private readonly Mock<IRepositorioEquipo> _mockRepoEquipo = new();
    private readonly Mock<IRepositorioOrdenDeTrabajo> _mockRepoOrden = new();
    private readonly Mock<IRepositorioBitacoraOrden> _mockRepoBitacora = new();
    private readonly IOptions<ReglasNegocioOptions> _options;

    public ServicioFallaYPrediccionTests()
    {
        _options = Options.Create(new ReglasNegocioOptions
        {
            MaxFallasParaRiesgo = 3,
            PlazoHorasAlta = 48,
            PlazoHorasMedia = 120
        });
    }

    [Fact]
    public async Task RegistrarAsync_TerceraFallaEnUltimos6Meses_CambiaEstadoEquipoAEnRiesgo_RN03()
    {
        // Arrange
        const string codigoEquipo = "EQ-001";
        var equipo = new Equipo { CodigoEquipo = codigoEquipo, Nombre = "Servidor ERP", IdEstado = 3 }; // 3 = Operativo
        var ordenActiva = new OrdenDeTrabajo { Numero = 20, CodigoEquipo = codigoEquipo, IdEstado = 8 };

        _mockRepoEquipo.Setup(r => r.ObtenerPorCodigoAsync(codigoEquipo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipo);
        _mockRepoOrden.Setup(r => r.ObtenerPorEquipoAsync(codigoEquipo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OrdenDeTrabajo> { ordenActiva });
        _mockRepoFalla.Setup(r => r.ContarFallasUltimosMesesAsync(codigoEquipo, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var servicio = new ServicioFalla(
            _mockRepoFalla.Object,
            _mockRepoEquipo.Object,
            _mockRepoOrden.Object,
            _mockRepoBitacora.Object,
            _options);

        var dto = new RegistrarFallaDto
        {
            CodigoEquipo = codigoEquipo,
            IdTipoGravedad = 3,
            Descripcion = "Falla recurrente en fuente de poder redundante"
        };

        // Act
        var resultado = await servicio.RegistrarAsync(dto);

        // Assert
        Assert.True(resultado.PasoAEnRiesgoAutomatico);
        Assert.Equal(5, equipo.IdEstado); // 5 = EnRiesgo
        _mockRepoEquipo.Verify(r => r.ActualizarAsync(It.Is<Equipo>(e => e.IdEstado == 5), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarAsync_MenosDeTresFallas_MantieneEstadoOperativo_RN03()
    {
        // Arrange
        const string codigoEquipo = "EQ-002";
        var equipo = new Equipo { CodigoEquipo = codigoEquipo, Nombre = "Robot Soldadura", IdEstado = 3 };
        var ordenActiva = new OrdenDeTrabajo { Numero = 21, CodigoEquipo = codigoEquipo, IdEstado = 9 };

        _mockRepoEquipo.Setup(r => r.ObtenerPorCodigoAsync(codigoEquipo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipo);
        _mockRepoOrden.Setup(r => r.ObtenerPorEquipoAsync(codigoEquipo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OrdenDeTrabajo> { ordenActiva });
        _mockRepoFalla.Setup(r => r.ContarFallasUltimosMesesAsync(codigoEquipo, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var servicio = new ServicioFalla(
            _mockRepoFalla.Object,
            _mockRepoEquipo.Object,
            _mockRepoOrden.Object,
            _mockRepoBitacora.Object,
            _options);

        var dto = new RegistrarFallaDto
        {
            CodigoEquipo = codigoEquipo,
            IdTipoGravedad = 2,
            Descripcion = "Desajuste leve en sensor óptico"
        };

        // Act
        var resultado = await servicio.RegistrarAsync(dto);

        // Assert
        Assert.False(resultado.PasoAEnRiesgoAutomatico);
        Assert.Equal(3, equipo.IdEstado);
        _mockRepoEquipo.Verify(r => r.ActualizarAsync(It.IsAny<Equipo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarAsync_EquipoDadoDeBaja_LanzaReglaNegocioException_RN04()
    {
        // Arrange
        const string codigoEquipo = "EQ-008";
        _mockRepoEquipo.Setup(r => r.ObtenerPorCodigoAsync(codigoEquipo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Equipo { CodigoEquipo = codigoEquipo, Nombre = "Compresor Antiguo", IdEstado = 6 });

        var servicio = new ServicioFalla(
            _mockRepoFalla.Object,
            _mockRepoEquipo.Object,
            _mockRepoOrden.Object,
            _mockRepoBitacora.Object,
            _options);

        var dto = new RegistrarFallaDto
        {
            CodigoEquipo = codigoEquipo,
            IdTipoGravedad = 3,
            Descripcion = "Falla en equipo dado de baja"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ReglaNegocioException>(() => servicio.RegistrarAsync(dto));
    }

    [Fact]
    public async Task RegistrarAsync_SinOrdenActiva_CreaOrdenCorrectivaYBitacoraAutomaticamente()
    {
        // Arrange
        const string codigoEquipo = "EQ-005";
        var equipo = new Equipo { CodigoEquipo = codigoEquipo, Nombre = "Generador Diesel", IdEstado = 3 };

        _mockRepoEquipo.Setup(r => r.ObtenerPorCodigoAsync(codigoEquipo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipo);
        _mockRepoOrden.Setup(r => r.ObtenerPorEquipoAsync(codigoEquipo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OrdenDeTrabajo>());
        _mockRepoFalla.Setup(r => r.ContarFallasUltimosMesesAsync(codigoEquipo, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var servicio = new ServicioFalla(
            _mockRepoFalla.Object,
            _mockRepoEquipo.Object,
            _mockRepoOrden.Object,
            _mockRepoBitacora.Object,
            _options);

        var dto = new RegistrarFallaDto
        {
            CodigoEquipo = codigoEquipo,
            IdTipoGravedad = 4,
            Descripcion = "Falla de arranque en generador de emergencia",
            IdentificacionSolicitante = "USR-005"
        };

        // Act
        await servicio.RegistrarAsync(dto);

        // Assert
        _mockRepoOrden.Verify(r => r.AgregarAsync(It.Is<OrdenDeTrabajo>(o => o.IdTipoOrden == 2 && o.Prioridad == "Critica"), It.IsAny<CancellationToken>()), Times.Once);
        _mockRepoBitacora.Verify(r => r.RegistrarAsync(It.IsAny<BitacoraOrden>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PredecirFallaEquipoAsync_EquipoConAltaReincidencia_RetornaProbabilidadMayorAEquipoSano()
    {
        // Arrange
        var servicioMl = new ServicioPrediccionFalla(_mockRepoEquipo.Object, _mockRepoFalla.Object, _mockRepoOrden.Object);

        _mockRepoEquipo.Setup(r => r.ObtenerPorCodigoAsync("EQ-SANO", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Equipo { CodigoEquipo = "EQ-SANO", Nombre = "Equipo Nuevo", FechaAdquisicion = DateTime.UtcNow.AddMonths(-6), IdEstado = 3 });
        _mockRepoFalla.Setup(r => r.ContarFallasUltimosMesesAsync("EQ-SANO", 6, It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _mockRepoFalla.Setup(r => r.ObtenerPorEquipoAsync("EQ-SANO", It.IsAny<CancellationToken>())).ReturnsAsync(new List<FallaRegistrada>());
        _mockRepoOrden.Setup(r => r.ObtenerPorEquipoAsync("EQ-SANO", It.IsAny<CancellationToken>())).ReturnsAsync(new List<OrdenDeTrabajo>());

        _mockRepoEquipo.Setup(r => r.ObtenerPorCodigoAsync("EQ-CRIT", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Equipo { CodigoEquipo = "EQ-CRIT", Nombre = "Equipo Crítico", FechaAdquisicion = DateTime.UtcNow.AddMonths(-72), IdEstado = 5 });
        _mockRepoFalla.Setup(r => r.ContarFallasUltimosMesesAsync("EQ-CRIT", 6, It.IsAny<CancellationToken>())).ReturnsAsync(5);
        _mockRepoFalla.Setup(r => r.ObtenerPorEquipoAsync("EQ-CRIT", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FallaRegistrada>
            {
                new() { IdTipoGravedad = 4 },
                new() { IdTipoGravedad = 4 },
                new() { IdTipoGravedad = 3 }
            });
        _mockRepoOrden.Setup(r => r.ObtenerPorEquipoAsync("EQ-CRIT", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OrdenDeTrabajo>
            {
                new() { IdTipoOrden = 2, TiempoInvertido = 20 },
                new() { IdTipoOrden = 2, TiempoInvertido = 25 },
                new() { IdTipoOrden = 2, TiempoInvertido = 15 }
            });

        // Act
        PrediccionFallaEquipoDto predSano = await servicioMl.PredecirFallaEquipoAsync("EQ-SANO");
        PrediccionFallaEquipoDto predCritico = await servicioMl.PredecirFallaEquipoAsync("EQ-CRIT");

        // Assert
        Assert.InRange(predSano.ProbabilidadFalla30Dias, 0.01f, 0.99f);
        Assert.InRange(predCritico.ProbabilidadFalla30Dias, 0.01f, 0.99f);
        Assert.True(predCritico.ProbabilidadFalla30Dias > predSano.ProbabilidadFalla30Dias);
        Assert.Equal("Alto", predCritico.NivelRiesgo);
    }

    [Fact]
    public async Task PredecirFallaEquipoAsync_EquipoInexistente_LanzaNoEncontradoException()
    {
        // Arrange
        _mockRepoEquipo.Setup(r => r.ObtenerPorCodigoAsync("EQ-NOEXISTE", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Equipo?)null);
        var servicioMl = new ServicioPrediccionFalla(_mockRepoEquipo.Object, _mockRepoFalla.Object, _mockRepoOrden.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NoEncontradoException>(() => servicioMl.PredecirFallaEquipoAsync("EQ-NOEXISTE"));
    }
}
