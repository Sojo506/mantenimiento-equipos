
using Abstracciones.DTOs.Usuarios;
using Abstracciones.Interfaces.Flujo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IServicioUsuario _servicioUsuario;

    public UsuariosController(IServicioUsuario servicioUsuario)
    {
        _servicioUsuario = servicioUsuario;
    }

    [HttpGet]
    [Authorize(Roles = "Administrador,Supervisor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodos(
        CancellationToken cancellationToken)
    {
        var usuarios = await _servicioUsuario.ObtenerTodosAsync(
            cancellationToken);

        return Ok(usuarios);
    }

    [HttpGet("buscar/{idUsuario}")]
    [Authorize(Roles = "Administrador,Supervisor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorIdentificacion(
        [FromRoute(Name = "idUsuario")] string identificacion,
        CancellationToken cancellationToken)
    {
        var usuario = await _servicioUsuario.ObtenerPorIdentificacionAsync(
            identificacion.Trim(),
            cancellationToken);

        if (usuario is null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el usuario {identificacion}."
            });
        }

        return Ok(usuario);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear(
        [FromBody] CrearUsuarioDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var usuario = await _servicioUsuario.CrearAsync(
            dto,
            cancellationToken);

        return CreatedAtAction(
            nameof(ObtenerPorIdentificacion),
            new { idUsuario = usuario.Identificacion },
            usuario);
    }

    [HttpPut("{identificacion}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(
        [FromRoute] string identificacion,
        [FromBody] ActualizarUsuarioDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _servicioUsuario.ActualizarAsync(
            identificacion,
            dto,
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{identificacion}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(
        [FromRoute] string identificacion,
        CancellationToken cancellationToken)
    {
        await _servicioUsuario.EliminarAsync(
            identificacion,
            cancellationToken);

        return NoContent();
    }
}
