using Abstracciones.Configuracion;
using Abstracciones.DTOs.Ordenes;
using Abstracciones.Entidades;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces.DA;
using Flujo.Servicios;
using Microsoft.Extensions.Options;
using Moq;

namespace Flujo.Tests.Servicios;

public class ServicioOrdenDeTrabajoReglasTests
{
    private const string UsuarioSistema = "USR-SYS";

    private readonly Mock<IRepositorioOrdenDeTrabajo> _repoOrden = new();
    private readonly Mock<IRepositorioEquipo> _repoEquipo = new();
    private readonly Mock<IRepositorioUsuario> _repoUsuario = new();
    private readonly Mock<IRepositorioRepuesto> _repoRepuesto = new();
    private readonly Mock<IRepositorioBitacoraOrden> _repoBitacora = new();
    private readonly List<BitacoraOrden> _bitacoraRegistrada = new();
    private readonly ServicioOrdenDeTrabajo _servicio;

    public ServicioOrdenDeTrabajoReglasTests()
    {
        _repoBitacora
            .Setup(b => b.RegistrarAsync(It.IsAny<BitacoraOrden>(), It.IsAny<CancellationToken>()))
            .Callback<BitacoraOrden, CancellationToken>((b, _) => _bitacoraRegistrada.Add(b))
            .Returns(Task.CompletedTask);

        var options = Options.Create(new ReglasNegocioOptions
        {
            MaxHorasCriticaSinAtender = 4,
            MaxOrdenesEnProcesoPorTecnico = 3,
            IdentificacionUsuarioSistema = UsuarioSistema
        });

        _servicio = new ServicioOrdenDeTrabajo(
            _repoOrden.Object, _repoEquipo.Object, _repoUsuario.Object,
            _repoRepuesto.Object, _repoBitacora.Object, options);
    }

    [Fact]
    public async Task EscalarCriticasSinAtenderAsync_CriticasVencidas_PasanAEscaladaYRegistranBitacoraDelSistema_RN01()
    {
        var abierta = new OrdenDeTrabajo { Numero = 201, Prioridad = "Critica", IdEstado = 7, FechaCreacion = DateTime.UtcNow.AddHours(-10) };
        var asignada = new OrdenDeTrabajo { Numero = 202, Prioridad = "Critica", IdEstado = 8, FechaCreacion = DateTime.UtcNow.AddHours(-5) };

        _repoOrden.Setup(r => r.ObtenerCriticasSinAtenderAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { abierta, asignada });

        var escaladas = await _servicio.EscalarCriticasSinAtenderAsync();

        Assert.Equal(new[] { 201, 202 }, escaladas);
        Assert.All(new[] { abierta, asignada }, o => Assert.Equal(12, o.IdEstado));

        Assert.Equal(2, _bitacoraRegistrada.Count);
        Assert.All(_bitacoraRegistrada, b =>
        {
            Assert.Equal(UsuarioSistema, b.Identificacion);
            Assert.Equal("Escalada", b.EstadoNuevo);
            Assert.Contains("RN-01", b.Comentario);
        });
        Assert.Equal("Abierta", _bitacoraRegistrada[0].EstadoAnterior);
        Assert.Equal("Asignada", _bitacoraRegistrada[1].EstadoAnterior);

        _repoOrden.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EscalarCriticasSinAtenderAsync_UsaComoCorteElMaximoDeHorasConfigurado_RN01()
    {
        DateTime? corteRecibido = null;
        _repoOrden.Setup(r => r.ObtenerCriticasSinAtenderAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .Callback<DateTime, CancellationToken>((corte, _) => corteRecibido = corte)
            .ReturnsAsync(Array.Empty<OrdenDeTrabajo>());

        var escaladas = await _servicio.EscalarCriticasSinAtenderAsync();

        Assert.Empty(escaladas);
        Assert.NotNull(corteRecibido);
        Assert.InRange(corteRecibido!.Value, DateTime.UtcNow.AddHours(-4).AddMinutes(-1), DateTime.UtcNow.AddHours(-4).AddMinutes(1));
        _repoOrden.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Never);
        Assert.Empty(_bitacoraRegistrada);
    }

    [Fact]
    public async Task CambiarEstadoAsync_CierreSinTiempoInvertido_LanzaReglaNegocioException_RN02()
    {
        var orden = new OrdenDeTrabajo { Numero = 210, IdEstado = 9, TiempoInvertido = 0, RowVersion = new byte[] { 1 } };
        _repoOrden.Setup(r => r.ObtenerPorNumeroAsync(210, It.IsAny<CancellationToken>())).ReturnsAsync(orden);

        var dto = new CambiarEstadoOrdenDto
        {
            NuevoEstado = 11,
            IdentificacionUsuario = "USR-003",
            Comentario = "Cierre",
            TiempoInvertidoFinal = 0,
            SinRepuestos = true,
            JustificacionSinRepuestos = "Solo inspección",
            RowVersion = Convert.ToBase64String(orden.RowVersion)
        };

        var ex = await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.CambiarEstadoAsync(210, dto));
        Assert.Contains("RN-02", ex.Message);
        Assert.Equal(9, orden.IdEstado);
        _repoOrden.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CambiarEstadoAsync_CierreConRepuestosUsados_NoExigeJustificacionYRegistraBitacora_RN02()
    {
        var orden = new OrdenDeTrabajo
        {
            Numero = 211,
            IdEstado = 9,
            RowVersion = new byte[] { 7 },
            RepuestosUsados = new List<OrdenRepuesto> { new() { CodigoRepuesto = "REP-001", CantidadUsada = 2 } }
        };
        _repoOrden.Setup(r => r.ObtenerPorNumeroAsync(211, It.IsAny<CancellationToken>())).ReturnsAsync(orden);

        var dto = new CambiarEstadoOrdenDto
        {
            NuevoEstado = 11,
            IdentificacionUsuario = "USR-003",
            Comentario = "Se reemplazaron filtros",
            TiempoInvertidoFinal = 2.5m,
            RowVersion = Convert.ToBase64String(orden.RowVersion)
        };

        await _servicio.CambiarEstadoAsync(211, dto);

        Assert.Equal(11, orden.IdEstado);
        Assert.Equal(2.5m, orden.TiempoInvertido);
        var evento = Assert.Single(_bitacoraRegistrada);
        Assert.Equal("En Proceso", evento.EstadoAnterior);
        Assert.Equal("Cerrada", evento.EstadoNuevo);
    }

    [Fact]
    public async Task CambiarEstadoAsync_OrdenYaCerrada_LanzaConflictoException_RN09()
    {
        var orden = new OrdenDeTrabajo { Numero = 212, IdEstado = 11, RowVersion = new byte[] { 3 } };
        _repoOrden.Setup(r => r.ObtenerPorNumeroAsync(212, It.IsAny<CancellationToken>())).ReturnsAsync(orden);

        var dto = new CambiarEstadoOrdenDto
        {
            NuevoEstado = 9,
            IdentificacionUsuario = "USR-002",
            Comentario = "Reabrir",
            RowVersion = Convert.ToBase64String(orden.RowVersion)
        };

        var ex = await Assert.ThrowsAsync<ConflictoException>(() => _servicio.CambiarEstadoAsync(212, dto));
        Assert.Contains("RN-09", ex.Message);
    }

    [Fact]
    public async Task AsignarRepuestoAsync_RowVersionDesactualizado_LanzaConflictoSinDescontarStock()
    {
        var orden = new OrdenDeTrabajo { Numero = 220, IdEstado = 9, RowVersion = new byte[] { 2 } };
        var repuesto = new Repuesto { CodigoRepuesto = "REP-001", Stock = 10, CostoUnitario = 5m };

        _repoOrden.Setup(r => r.ObtenerPorNumeroAsync(220, It.IsAny<CancellationToken>())).ReturnsAsync(orden);
        _repoRepuesto.Setup(r => r.ObtenerPorCodigoAsync("REP-001", It.IsAny<CancellationToken>())).ReturnsAsync(repuesto);

        var dto = new AsignarRepuestoOrdenDto
        {
            CodigoRepuesto = "REP-001",
            Cantidad = 3,
            RowVersion = Convert.ToBase64String(new byte[] { 1 })
        };

        await Assert.ThrowsAsync<ConflictoException>(() => _servicio.AsignarRepuestoAsync(220, dto));
        Assert.Equal(10, repuesto.Stock);
        Assert.Empty(orden.RepuestosUsados);
        _repoOrden.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AsignarRepuestoAsync_RowVersionVigente_DescuentaStockYAgregaRepuesto_RN05()
    {
        var orden = new OrdenDeTrabajo { Numero = 221, IdEstado = 9, RowVersion = new byte[] { 4 } };
        var repuesto = new Repuesto { CodigoRepuesto = "REP-002", Stock = 10, CostoUnitario = 32.5m };

        _repoOrden.Setup(r => r.ObtenerPorNumeroAsync(221, It.IsAny<CancellationToken>())).ReturnsAsync(orden);
        _repoRepuesto.Setup(r => r.ObtenerPorCodigoAsync("REP-002", It.IsAny<CancellationToken>())).ReturnsAsync(repuesto);

        await _servicio.AsignarRepuestoAsync(221, new AsignarRepuestoOrdenDto
        {
            CodigoRepuesto = "REP-002",
            Cantidad = 4,
            RowVersion = Convert.ToBase64String(orden.RowVersion)
        });

        Assert.Equal(6, repuesto.Stock);
        var usado = Assert.Single(orden.RepuestosUsados);
        Assert.Equal(4, usado.CantidadUsada);
        Assert.Equal(32.5m, usado.CostoAplicado);
    }

    [Fact]
    public async Task AsignarTecnicoAsync_OrdenEscalada_PasaAAsignadaConElNuevoTecnico()
    {
        var orden = new OrdenDeTrabajo { Numero = 230, IdEstado = 12, IdentificacionTecnico = "USR-003", RowVersion = new byte[] { 9 } };
        var tecnico = new Usuario { Identificacion = "USR-004", Nombre = "Mariana", IdTipoUsuario = 3, IdEstado = 1 };

        _repoOrden.Setup(r => r.ObtenerPorNumeroAsync(230, It.IsAny<CancellationToken>())).ReturnsAsync(orden);
        _repoUsuario.Setup(r => r.ObtenerPorIdentificacionAsync("USR-004", It.IsAny<CancellationToken>())).ReturnsAsync(tecnico);

        await _servicio.AsignarTecnicoAsync(230, new AsignarTecnicoOrdenDto
        {
            IdentificacionTecnico = "USR-004",
            IdentificacionUsuario = "USR-002",
            RowVersion = Convert.ToBase64String(orden.RowVersion)
        });

        Assert.Equal(8, orden.IdEstado);
        Assert.Equal("USR-004", orden.IdentificacionTecnico);
        var evento = Assert.Single(_bitacoraRegistrada);
        Assert.Equal("Escalada", evento.EstadoAnterior);
        Assert.Equal("Asignada", evento.EstadoNuevo);
        Assert.Equal("USR-002", evento.Identificacion);
    }

    [Fact]
    public async Task AsignarTecnicoAsync_UsuarioQueNoEsTecnico_LanzaReglaNegocioException()
    {
        var orden = new OrdenDeTrabajo { Numero = 231, IdEstado = 7, RowVersion = new byte[] { 1 } };
        var solicitante = new Usuario { Identificacion = "USR-005", IdTipoUsuario = 4, IdEstado = 1 };

        _repoOrden.Setup(r => r.ObtenerPorNumeroAsync(231, It.IsAny<CancellationToken>())).ReturnsAsync(orden);
        _repoUsuario.Setup(r => r.ObtenerPorIdentificacionAsync("USR-005", It.IsAny<CancellationToken>())).ReturnsAsync(solicitante);

        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.AsignarTecnicoAsync(231, new AsignarTecnicoOrdenDto
        {
            IdentificacionTecnico = "USR-005",
            IdentificacionUsuario = "USR-002",
            RowVersion = Convert.ToBase64String(orden.RowVersion)
        }));

        Assert.Equal(7, orden.IdEstado);
        Assert.Null(orden.IdentificacionTecnico);
    }
}
