using Abstracciones.DTOs.Equipos;
using Abstracciones.Entidades;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces.DA;
using Flujo.Servicios;
using Moq;

namespace Flujo.Tests.Servicios;

/// <summary>
/// Pruebas unitarias para la lógica de administración de equipos.
/// </summary>
public class ServicioEquipoTests
{
    private readonly Mock<IRepositorioEquipo> _mockRepositorioEquipo;
    private readonly Mock<IRepositorioOrdenDeTrabajo> _mockRepositorioOrden;
    private readonly ServicioEquipo _servicio;

    public ServicioEquipoTests()
    {
        _mockRepositorioEquipo = new Mock<IRepositorioEquipo>();
        _mockRepositorioOrden = new Mock<IRepositorioOrdenDeTrabajo>();
        _servicio = new ServicioEquipo(_mockRepositorioEquipo.Object, _mockRepositorioOrden.Object);
    }

    [Fact]
    public async Task CrearAsync_CodigoDuplicado_LanzaConflictoException()
    {
        // Arrange
        const string codigo = "EQ-PREN-001";
        _mockRepositorioEquipo.Setup(r => r.ExisteCodigoAsync(codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var dto = new CrearEquipoDto
        {
            CodigoEquipo = codigo,
            Nombre = "Prensa Hidráulica",
            IdTipoEquipo = 1,
            IdMarca = 1,
            IdModelo = 1,
            IdUbicacion = 1
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.CrearAsync(dto));
        Assert.Contains("Ya existe un equipo registrado", ex.Message);
    }

    [Fact]
    public async Task EliminarAsync_ConOrdenesAsociadas_LanzaReglaNegocioException()
    {
        // Arrange
        const string codigo = "EQ-COMP-002";
        var equipo = new Equipo { CodigoEquipo = codigo, Nombre = "Compresor" };

        _mockRepositorioEquipo.Setup(r => r.ObtenerPorCodigoAsync(codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipo);
        _mockRepositorioOrden.Setup(r => r.ObtenerPorEquipoAsync(codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OrdenDeTrabajo> { new() { Numero = 1 } });

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.EliminarAsync(codigo));
        Assert.Contains("órdenes de trabajo registradas", ex.Message);
    }

    [Fact]
    public async Task ObtenerPorCodigoAsync_NoExistente_LanzaNoEncontradoException()
    {
        // Arrange
        const string codigo = "EQ-INEXISTENTE";
        _mockRepositorioEquipo.Setup(r => r.ObtenerPorCodigoAsync(codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Equipo?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NoEncontradoException>(() => _servicio.ObtenerPorCodigoAsync(codigo));
    }
}
