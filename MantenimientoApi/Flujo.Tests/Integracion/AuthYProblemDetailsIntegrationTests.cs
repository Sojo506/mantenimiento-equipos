using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Abstracciones.DTOs.Equipos;
using Abstracciones.DTOs.Usuarios;
using Abstracciones.Entidades;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Seguridad;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Flujo.Tests.Integracion;

/// <summary>
/// Pruebas de integración con WebApplicationFactory para verificar autenticación JWT,
/// autorización por roles (401/403/200) y formato estándar de errores RFC 7807 (application/problem+json).
/// </summary>
public class AuthYProblemDetailsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IRepositorioUsuario> _mockRepoUsuario = new();
    private readonly Mock<IServicioEquipo> _mockServicioEquipo = new();

    public AuthYProblemDetailsIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IRepositorioUsuario>();
                services.AddScoped(_ => _mockRepoUsuario.Object);

                services.RemoveAll<IServicioEquipo>();
                services.AddScoped(_ => _mockServicioEquipo.Object);
            });
        });
    }

    [Fact]
    public async Task EndpointProtegido_SinTokenJwt_Retorna401Unauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/equipos");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_CredencialesIncorrectas_Retorna401ProblemDetailsRfc7807()
    {
        // Arrange
        _mockRepoUsuario.Setup(r => r.ObtenerPorCorreoAsync("inexistente@mantenimiento.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        using var client = _factory.CreateClient();
        var loginDto = new LoginDto
        {
            Correo = "inexistente@mantenimiento.com",
            Password = "WrongPassword123!"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.Equal(401, doc.RootElement.GetProperty("status").GetInt32());
        Assert.Contains("Correo o contraseña incorrectos", doc.RootElement.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task LoginExitosoYControlDeRoles_SolicitanteNoPuedeCrearEquipo_Retorna403Forbidden()
    {
        // Arrange
        var usuarioSolicitante = new Usuario
        {
            Identificacion = "USR-005",
            Nombre = "María",
            PrimerApellido = "Solano",
            Correo = "solicitante@mantenimiento.com",
            PasswordHash = HashHelper.Hashear("Solicitante123!"),
            IdTipoUsuario = 4, // Solicitante
            TipoUsuario = new TipoUsuario { IdTipoUsuario = 4, Nombre = "Solicitante" },
            Disponibilidad = true,
            IdEstado = 1
        };

        _mockRepoUsuario.Setup(r => r.ObtenerPorCorreoAsync("solicitante@mantenimiento.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuarioSolicitante);

        using var client = _factory.CreateClient();

        // 1. Login para obtener JWT con rol Solicitante
        var loginResp = await client.PostAsJsonAsync("/api/auth/login", new LoginDto
        {
            Correo = "solicitante@mantenimiento.com",
            Password = "Solicitante123!"
        });
        Assert.Equal(HttpStatusCode.OK, loginResp.StatusCode);

        var loginData = await loginResp.Content.ReadFromJsonAsync<RespuestaLoginDto>();
        Assert.NotNull(loginData);
        Assert.False(string.IsNullOrWhiteSpace(loginData!.Token));

        // 2. Intentar POST /api/equipos (requiere Administrador o Supervisor)
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Token);
        var postResp = await client.PostAsJsonAsync("/api/equipos", new CrearEquipoDto
        {
            CodigoEquipo = "EQ-900",
            Nombre = "Equipo No Autorizado",
            IdTipoEquipo = 1,
            IdMarca = 1,
            IdModelo = 1,
            IdUbicacion = 1,
            IdEstado = 3
        });

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, postResp.StatusCode);
    }

    [Fact]
    public async Task ExcepcionDeDominio_NoEncontrado_Retorna404ProblemDetailsEstandar()
    {
        // Arrange
        var admin = new Usuario
        {
            Identificacion = "USR-001",
            Nombre = "Carlos",
            PrimerApellido = "Mora",
            Correo = "admin@mantenimiento.com",
            PasswordHash = HashHelper.Hashear("Admin123!"),
            IdTipoUsuario = 1,
            TipoUsuario = new TipoUsuario { IdTipoUsuario = 1, Nombre = "Administrador" },
            Disponibilidad = true,
            IdEstado = 1
        };

        _mockRepoUsuario.Setup(r => r.ObtenerPorCorreoAsync("admin@mantenimiento.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(admin);

        _mockServicioEquipo.Setup(s => s.ObtenerPorCodigoAsync("EQ-404", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NoEncontradoException("Equipo", "EQ-404"));

        using var client = _factory.CreateClient();
        var loginResp = await client.PostAsJsonAsync("/api/auth/login", new LoginDto
        {
            Correo = "admin@mantenimiento.com",
            Password = "Admin123!"
        });
        var loginData = await loginResp.Content.ReadFromJsonAsync<RespuestaLoginDto>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginData!.Token);

        // Act
        var resp = await client.GetAsync("/api/equipos/EQ-404");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        Assert.Equal("application/problem+json", resp.Content.Headers.ContentType?.MediaType);
        var json = await resp.Content.ReadAsStringAsync();
        Assert.Contains("EQ-404", json);
    }
}
