using Abstracciones.DTOs.Repuestos;
using Abstracciones.Entidades;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces.DA;
using Flujo.Servicios;
using Moq;

namespace Flujo.Tests.Servicios;

/// <summary>
/// Pruebas unitarias para la lógica de negocio de repuestos e inventario (RN-05 y concurrencia optimista).
/// </summary>
public class ServicioRepuestoTests
{
    private readonly Mock<IRepositorioRepuesto> _mockRepositorio;
    private readonly ServicioRepuesto _servicio;

    public ServicioRepuestoTests()
    {
        _mockRepositorio = new Mock<IRepositorioRepuesto>();
        _servicio = new ServicioRepuesto(_mockRepositorio.Object);
    }

    [Fact]
    public async Task CrearAsync_CodigoDuplicado_LanzaReglaNegocioException()
    {
        // Arrange
        const string codigo = "REP-FIL-001";
        _mockRepositorio.Setup(r => r.ExisteCodigoAsync(codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var dto = new CrearRepuestoDto
        {
            CodigoRepuesto = codigo,
            Nombre = "Filtro de Aceite",
            Stock = 10,
            StockMinimo = 2,
            CostoUnitario = 25.50m
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.CrearAsync(dto));
        Assert.Contains("Ya existe un repuesto registrado", ex.Message);
    }

    [Fact]
    public async Task ActualizarAsync_ConcurrenciaDesactualizada_LanzaConflictoException()
    {
        // Arrange
        const string codigo = "REP-VAL-002";
        var repuesto = new Repuesto
        {
            CodigoRepuesto = codigo,
            Nombre = "Válvula de Presión",
            Stock = 15,
            StockMinimo = 3,
            RowVersion = new byte[] { 1, 2, 3 }
        };

        _mockRepositorio.Setup(r => r.ObtenerPorCodigoAsync(codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(repuesto);

        var dto = new ActualizarRepuestoDto
        {
            Nombre = "Válvula Modificada",
            Stock = 10,
            StockMinimo = 3,
            CostoUnitario = 45.00m,
            IdEstado = 1,
            RowVersion = Convert.ToBase64String(new byte[] { 9, 8, 7 }) // Token viejo
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ConflictoException>(() => _servicio.ActualizarAsync(codigo, dto));
        Assert.Contains("modificado concurrentemente", ex.Message);
    }

    [Fact]
    public async Task ActualizarAsync_StockNegativo_LanzaReglaNegocioException_RN05()
    {
        // Arrange
        const string codigo = "REP-VAL-002";
        var repuesto = new Repuesto
        {
            CodigoRepuesto = codigo,
            Nombre = "Válvula de Presión",
            Stock = 15,
            StockMinimo = 3
        };

        _mockRepositorio.Setup(r => r.ObtenerPorCodigoAsync(codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(repuesto);

        var dto = new ActualizarRepuestoDto
        {
            Nombre = "Válvula Modificada",
            Stock = -5,
            StockMinimo = 3,
            CostoUnitario = 45.00m,
            IdEstado = 1
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.ActualizarAsync(codigo, dto));
        Assert.Contains("negativo", ex.Message);
    }

    [Fact]
    public async Task EliminarAsync_NoExistente_LanzaNoEncontradoException()
    {
        // Arrange
        const string codigo = "REP-INEXISTENTE";
        _mockRepositorio.Setup(r => r.ObtenerPorCodigoAsync(codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Repuesto?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NoEncontradoException>(() => _servicio.EliminarAsync(codigo));
    }
}
