using System.Text;
using Abstracciones.DTOs.Reportes;
using Abstracciones.DTOs.Usuarios;
using Abstracciones.Entidades;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces;
using Abstracciones.Interfaces.DA;
using Flujo.Servicios;
using Moq;

namespace Flujo.Tests.Servicios;

/// <summary>
/// Pruebas unitarias para ServicioUsuario y ServicioReportes (generación CSV y validaciones).
/// </summary>
public class ServicioUsuarioYReportesTests
{
    private readonly Mock<IRepositorioUsuario> _mockRepoUsuario = new();
    private readonly Mock<IServicioPasswordHasher> _mockHasher = new();
    private readonly Mock<IRepositorioReportes> _mockRepoReportes = new();

    [Fact]
    public async Task CrearUsuario_CorreoDuplicado_LanzaReglaNegocioException()
    {
        // Arrange
        _mockRepoUsuario.Setup(r => r.ObtenerPorCorreoAsync("admin@mantenimiento.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Usuario { Identificacion = "USR-001", Correo = "admin@mantenimiento.com" });

        var servicio = new ServicioUsuario(_mockRepoUsuario.Object, _mockHasher.Object);

        var dto = new CrearUsuarioDto
        {
            Identificacion = "USR-999",
            Nombre = "Pedro",
            PrimerApellido = "Mora",
            Correo = "admin@mantenimiento.com",
            Password = "Password123!",
            IdTipoUsuario = 3,
            IdEstado = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ReglaNegocioException>(() => servicio.CrearAsync(dto));
    }

    [Fact]
    public async Task EliminarUsuario_Inexistente_LanzaNoEncontradoException()
    {
        // Arrange
        _mockRepoUsuario.Setup(r => r.ObtenerPorIdentificacionAsync("USR-404", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        var servicio = new ServicioUsuario(_mockRepoUsuario.Object, _mockHasher.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NoEncontradoException>(() => servicio.EliminarAsync("USR-404"));
    }

    [Fact]
    public async Task GenerarCsvConsumoRepuestosAsync_RangoFechasInvalido_LanzaReglaNegocioException()
    {
        // Arrange
        var servicio = new ServicioReportes(_mockRepoReportes.Object);
        var inicio = new DateTime(2026, 12, 1);
        var fin = new DateTime(2026, 1, 1);

        // Act & Assert
        await Assert.ThrowsAsync<ReglaNegocioException>(() => servicio.GenerarCsvConsumoRepuestosAsync(inicio, fin));
    }

    [Fact]
    public async Task GenerarCsvConsumoRepuestosAsync_DatosValidos_GeneraCsvConEncabezadosYFilas()
    {
        // Arrange
        var inicio = new DateTime(2026, 1, 1);
        var fin = new DateTime(2026, 12, 31);

        _mockRepoReportes.Setup(r => r.ObtenerConsumoRepuestosPorPeriodoAsync(inicio, fin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ReporteConsumoRepuestoDto>
            {
                new()
                {
                    CodigoRepuesto = "REP-001",
                    NombreRepuesto = "Filtro de Aire, Alta Eficiencia",
                    CantidadTotalConsumida = 12,
                    CostoTotalAcumulado = 540.00m,
                    NumeroDeOrdenes = 4
                }
            });

        var servicio = new ServicioReportes(_mockRepoReportes.Object);

        // Act
        byte[] csvBytes = await servicio.GenerarCsvConsumoRepuestosAsync(inicio, fin);
        string contenido = Encoding.UTF8.GetString(csvBytes);

        // Assert
        Assert.Contains("CodigoRepuesto,NombreRepuesto,CantidadTotalConsumida,CostoTotalAcumulado,NumeroDeOrdenes", contenido);
        Assert.Contains("REP-001,\"Filtro de Aire, Alta Eficiencia\",12,540.00,4", contenido);
    }

    [Fact]
    public async Task GenerarCsvEquiposCriticosAsync_IncluyeEquiposYReincidenciaFallas()
    {
        // Arrange
        _mockRepoReportes.Setup(r => r.ObtenerReporteEquiposCriticosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ReporteEquipoCriticoDto>
            {
                new()
                {
                    CodigoEquipo = "EQ-004",
                    NombreEquipo = "Compresor Línea 2",
                    TipoEquipo = "Compresor",
                    Ubicacion = "Taller",
                    EstadoActual = "En Riesgo",
                    TotalFallasRegistradas = 4,
                    OrdenesAbiertas = 1,
                    OrdenesCerradas = 5,
                    HorasMantenimientoTotal = 28.5m
                }
            });

        var servicio = new ServicioReportes(_mockRepoReportes.Object);

        // Act
        byte[] csvBytes = await servicio.GenerarCsvEquiposCriticosAsync();
        string contenido = Encoding.UTF8.GetString(csvBytes);

        // Assert
        Assert.Contains("CodigoEquipo,NombreEquipo,TipoEquipo,Ubicacion,EstadoActual,TotalFallasRegistradas", contenido);
        Assert.Contains("EQ-004,Compresor Línea 2,Compresor,Taller,En Riesgo,4,1,5,28.50", contenido);
    }
}
