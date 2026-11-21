using Abstracciones.Interfaces.Flujo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/salud")]
[Produces("application/json")]
[Tags("Diagnóstico")]
[AllowAnonymous]
public class SaludController : ControllerBase
{
    private readonly IServicioSalud _servicioSalud;
    private readonly IWebHostEnvironment _environment;

    public SaludController(
        IServicioSalud servicioSalud,
        IWebHostEnvironment environment)
    {
        _servicioSalud = servicioSalud;
        _environment = environment;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerEstado(
        CancellationToken cancellationToken)
    {
        bool baseDatosConectada =
            await _servicioSalud.VerificarConexionAsync(
                cancellationToken);

        return Ok(new
        {
            Estado = "Operativo",
            Version = "1.0.0 (Avance 1)",
            BaseDatos = baseDatosConectada
                ? "Conectada"
                : "Desconectada",
            FechaUtc = DateTime.UtcNow,
            Entorno = _environment.EnvironmentName
        });
    }
}