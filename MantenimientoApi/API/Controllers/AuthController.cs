using Abstracciones.DTOs.Usuarios;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces.Flujo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador de autenticación y emisión de tokens JWT delegando a la capa Flujo.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthServicio _authServicio;

    public AuthController(IAuthServicio authServicio)
    {
        _authServicio = authServicio;
    }

    /// <summary>
    /// Valida las credenciales del usuario y emite un token JWT firmado con su rol e identificación.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(RespuestaLoginDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var respuesta = await _authServicio.LoginAsync(dto);
        if (respuesta == null)
        {
            return Problem(
                detail: "Correo o contraseña incorrectos.",
                title: "Credenciales inválidas",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        return Ok(respuesta);
    }
}
