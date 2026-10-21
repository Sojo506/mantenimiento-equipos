using Abstracciones.Configuracion;
using Abstracciones.DTOs.Ordenes;
using Abstracciones.Entidades;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces.DA;
using Flujo.Servicios;
using Microsoft.Extensions.Options;
using Moq;

namespace Flujo.Tests.Servicios;

/// <summary>
/// Pruebas unitarias para la lógica de negocio de órdenes de trabajo (RN-02, RN-04, RN-05, RN-07, RN-08, RN-09).
/// </summary>
public class ServicioOrdenDeTrabajoTests
{
    private readonly Mock<IRepositorioOrdenDeTrabajo> _mockRepositorioOrden;
    private readonly Mock<IRepositorioEquipo> _mockRepositorioEquipo;
    private readonly Mock<IRepositorioUsuario> _mockRepositorioUsuario;
    private readonly Mock<IRepositorioRepuesto> _mockRepositorioRepuesto;
    private readonly Mock<IRepositorioBitacoraOrden> _mockRepositorioBitacora;
    private readonly IOptions<ReglasNegocioOptions> _options;
    private readonly ServicioOrdenDeTrabajo _servicio;

    public ServicioOrdenDeTrabajoTests()
    {
        _mockRepositorioOrden = new Mock<IRepositorioOrdenDeTrabajo>();
        _mockRepositorioEquipo = new Mock<IRepositorioEquipo>();
        _mockRepositorioUsuario = new Mock<IRepositorioUsuario>();
        _mockRepositorioRepuesto = new Mock<IRepositorioRepuesto>();
        _mockRepositorioBitacora = new Mock<IRepositorioBitacoraOrden>();

        _options = Options.Create(new ReglasNegocioOptions
        {
            MaxHorasCriticaSinAtender = 4,
            MaxFallasParaRiesgo = 3,
            MaxOrdenesEnProcesoPorTecnico = 3,
            PlazoHorasBaja = 240,
            PlazoHorasMedia = 120,
            PlazoHorasAlta = 48,
            PlazoHorasCritica = 24
        });

        _servicio = new ServicioOrdenDeTrabajo(
            _mockRepositorioOrden.Object,
            _mockRepositorioEquipo.Object,
            _mockRepositorioUsuario.Object,
            _mockRepositorioRepuesto.Object,
            _mockRepositorioBitacora.Object,
            _options);
    }

    [Fact]
    public async Task CrearAsync_CorrectivaConOrdenAbiertaExistente_LanzaReglaNegocioException_RN04()
    {
        // Arrange
        const string codigoEquipo = "EQ-TORN-001";
        const string solicitanteId = "112340567";

        var equipo = new Equipo { CodigoEquipo = codigoEquipo, Nombre = "Torno", IdEstado = 1 };

        _mockRepositorioEquipo.Setup(r => r.ObtenerPorCodigoAsync(codigoEquipo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipo);
        _mockRepositorioOrden.Setup(r => r.TieneOrdenCorrectivaAbiertaAsync(codigoEquipo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var dto = new CrearOrdenDto
        {
            CodigoEquipo = codigoEquipo,
            IdentificacionSolicitante = solicitanteId,
            IdTipoOrden = 2, // 2: Correctiva
            Prioridad = "Alta",
            Descripcion = "Falla de motor principal"
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.CrearAsync(dto));
        Assert.Contains("RN-04", ex.Message);
    }

    [Fact]
    public async Task CrearAsync_EquipoDadoDeBaja_LanzaReglaNegocioException_RN04()
    {
        // Arrange
        const string codigoEquipo = "EQ-OBSO-999";
        var equipo = new Equipo { CodigoEquipo = codigoEquipo, Nombre = "Equipo Viejo", IdEstado = 6 }; // 6 = DadoDeBaja

        _mockRepositorioEquipo.Setup(r => r.ObtenerPorCodigoAsync(codigoEquipo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipo);

        var dto = new CrearOrdenDto
        {
            CodigoEquipo = codigoEquipo,
            IdentificacionSolicitante = "112340567",
            IdTipoOrden = 1,
            Prioridad = "Baja",
            Descripcion = "Intento de orden a equipo dado de baja"
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.CrearAsync(dto));
        Assert.Contains("Dado de Baja", ex.Message);
    }

    [Fact]
    public async Task CambiarEstadoAsync_TransicionInvalidaDeAbiertaAEnProcesoDirecto_LanzaConflictoException_RN08()
    {
        // Arrange: Estado 7 (Abierta) solo puede ir a 8 (Asignada), 11 (Cerrada) o 12 (Escalada)
        int numeroOrden = 101;
        var orden = new OrdenDeTrabajo
        {
            Numero = numeroOrden,
            IdEstado = 7, // Abierta
            RowVersion = new byte[] { 1, 2, 3 }
        };

        _mockRepositorioOrden.Setup(r => r.ObtenerPorNumeroAsync(numeroOrden, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orden);

        var dto = new CambiarEstadoOrdenDto
        {
            NuevoEstado = 9, // En Proceso (inválido desde Abierta)
            IdentificacionUsuario = "112340567",
            Comentario = "Intento saltar diagnóstico",
            RowVersion = Convert.ToBase64String(orden.RowVersion)
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ConflictoException>(() => _servicio.CambiarEstadoAsync(numeroOrden, dto));
        Assert.Contains("Transición de estado inválida", ex.Message);
    }

    [Fact]
    public async Task CambiarEstadoAsync_ACerradaSinRepuestosNiJustificacion_LanzaReglaNegocioException_RN02()
    {
        // Arrange
        int numeroOrden = 102;
        var orden = new OrdenDeTrabajo
        {
            Numero = numeroOrden,
            IdEstado = 9, // En Proceso
            TiempoInvertido = 3.5m,
            RepuestosUsados = new List<OrdenRepuesto>(),
            SinRepuestos = false,
            RowVersion = new byte[] { 1, 2, 3 }
        };

        _mockRepositorioOrden.Setup(r => r.ObtenerPorNumeroAsync(numeroOrden, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orden);

        var dto = new CambiarEstadoOrdenDto
        {
            NuevoEstado = 11, // Cerrada
            IdentificacionUsuario = "112340567",
            Comentario = "Cierre de orden",
            TiempoInvertidoFinal = 3.5m,
            SinRepuestos = false,
            RowVersion = Convert.ToBase64String(orden.RowVersion)
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.CambiarEstadoAsync(numeroOrden, dto));
        Assert.Contains("sin repuestos", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CambiarEstadoAsync_ACerradaConJustificacionValida_PermiteTransicion_RN02()
    {
        // Arrange
        int numeroOrden = 103;
        var orden = new OrdenDeTrabajo
        {
            Numero = numeroOrden,
            IdEstado = 9, // En Proceso
            TiempoInvertido = 4.0m,
            RepuestosUsados = new List<OrdenRepuesto>(),
            RowVersion = new byte[] { 1, 2, 3 }
        };

        _mockRepositorioOrden.Setup(r => r.ObtenerPorNumeroAsync(numeroOrden, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orden);

        var dto = new CambiarEstadoOrdenDto
        {
            NuevoEstado = 11, // Cerrada
            IdentificacionUsuario = "112340567",
            Comentario = "Orden finalizada con éxito",
            TiempoInvertidoFinal = 4.0m,
            SinRepuestos = true,
            JustificacionSinRepuestos = "Ajuste de tuercas y lubricación general sin reemplazo de componentes",
            RowVersion = Convert.ToBase64String(orden.RowVersion)
        };

        // Act
        var resultado = await _servicio.CambiarEstadoAsync(numeroOrden, dto);

        // Assert
        Assert.NotNull(resultado);
        _mockRepositorioBitacora.Verify(b => b.RegistrarAsync(It.IsAny<BitacoraOrden>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockRepositorioOrden.Verify(o => o.ActualizarAsync(It.IsAny<OrdenDeTrabajo>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ActualizarAsync_OrdenCerradaEsInmutable_LanzaConflictoException_RN09()
    {
        // Arrange: Estado 11 es Cerrada
        int numeroOrden = 104;
        var orden = new OrdenDeTrabajo
        {
            Numero = numeroOrden,
            IdEstado = 11, // Cerrada
            RowVersion = new byte[] { 1, 2, 3 }
        };

        _mockRepositorioOrden.Setup(r => r.ObtenerPorNumeroAsync(numeroOrden, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orden);

        var dto = new ActualizarOrdenDto
        {
            Descripcion = "Intento de modificación a orden archivada",
            Prioridad = "Baja",
            RowVersion = Convert.ToBase64String(orden.RowVersion)
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ConflictoException>(() => _servicio.ActualizarAsync(numeroOrden, dto));
        Assert.Contains("RN-09", ex.Message);
    }

    [Fact]
    public async Task AsignarRepuestoAsync_StockInsuficiente_LanzaReglaNegocioException_RN05()
    {
        // Arrange
        int numeroOrden = 105;
        const string codigoRepuesto = "REP-ROD-6204";

        var orden = new OrdenDeTrabajo
        {
            Numero = numeroOrden,
            IdEstado = 9, // En Proceso
            RepuestosUsados = new List<OrdenRepuesto>()
        };

        var repuesto = new Repuesto
        {
            CodigoRepuesto = codigoRepuesto,
            Nombre = "Rodamiento 6204",
            Stock = 2, // Solo hay 2 unidades en bodega
            StockMinimo = 5,
            CostoUnitario = 15.00m
        };

        _mockRepositorioOrden.Setup(r => r.ObtenerPorNumeroAsync(numeroOrden, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orden);
        _mockRepositorioRepuesto.Setup(r => r.ObtenerPorCodigoAsync(codigoRepuesto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(repuesto);

        var dto = new AsignarRepuestoOrdenDto
        {
            CodigoRepuesto = codigoRepuesto,
            Cantidad = 10 // Pide 10 unidades cuando solo hay 2
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.AsignarRepuestoAsync(numeroOrden, dto));
        Assert.Contains("Stock insuficiente", ex.Message);
    }

    [Fact]
    public async Task CambiarEstadoAsync_TokenConcurrenciaDiferente_LanzaConflictoException()
    {
        // Arrange
        int numeroOrden = 106;
        var orden = new OrdenDeTrabajo
        {
            Numero = numeroOrden,
            IdEstado = 7,
            RowVersion = new byte[] { 1, 1, 1 } // Versión en DB
        };

        _mockRepositorioOrden.Setup(r => r.ObtenerPorNumeroAsync(numeroOrden, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orden);

        var dto = new CambiarEstadoOrdenDto
        {
            NuevoEstado = 8,
            IdentificacionUsuario = "112340567",
            Comentario = "Cambio con token desfasado",
            RowVersion = Convert.ToBase64String(new byte[] { 9, 9, 9 }) // Token desactualizado
        };

        // Act & Assert
        await Assert.ThrowsAsync<ConflictoException>(() => _servicio.CambiarEstadoAsync(numeroOrden, dto));
    }
}
