
using Abstracciones.DTOs.Usuarios;
using Abstracciones.Seguridad;
using DA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly MantenimientoDbContext _db;
        private readonly IConfiguration _configuration;

        public AuthController(
            MantenimientoDbContext db,
            IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.Correo) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new
                {
                    mensaje = "Debe ingresar el correo y la contraseña."
                });
            }

            var correo = dto.Correo.Trim().ToLowerInvariant();

            var usuario = await _db.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    u => u.Correo.ToLower() == correo,
                    cancellationToken);

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    mensaje = "Correo o contraseña incorrectos."
                });
            }

            if (usuario.IdEstado != 1 || !usuario.Disponibilidad)
            {
                return Unauthorized(new
                {
                    mensaje = "El usuario está inactivo o no disponible."
                });
            }

            // Verificar contraseña usando el mismo algoritmo
            // PBKDF2 SHA-256 utilizado al crear el usuario.
            bool passwordValida = HashHelper.Verificar(
                dto.Password,
                usuario.PasswordHash
            );

            if (!passwordValida)
            {
                return Unauthorized(new
                {
                    mensaje = "Correo o contraseña incorrectos."
                });
            }

            var rol = ObtenerRol(usuario.IdTipoUsuario);

            if (rol == null)
            {
                return Unauthorized(new
                {
                    mensaje = "El usuario no tiene un rol válido."
                });
            }

            var token = GenerarToken(
                usuario.Identificacion,
                usuario.Correo,
                usuario.Nombre,
                rol
            );

            var expiracion = DateTime.UtcNow
                .AddMinutes(ObtenerDuracionToken());

            return Ok(new
            {
                mensaje = "Inicio de sesión exitoso.",
                token,
                tipoToken = "Bearer",
                expiracion,

                usuario = new
                {
                    identificacion = usuario.Identificacion,
                    correo = usuario.Correo,
                    nombre = usuario.Nombre,
                    primerApellido = usuario.PrimerApellido,
                    segundoApellido = usuario.SegundoApellido,
                    rol
                }
            });
        }

        private string GenerarToken(
            string identificacion,
            string correo,
            string nombre,
            string rol)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];

            if (string.IsNullOrWhiteSpace(secretKey) ||
                Encoding.UTF8.GetByteCount(secretKey) < 32)
            {
                throw new InvalidOperationException(
                    "La clave JWT debe tener al menos 32 bytes."
                );
            }

            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, identificacion),
                new Claim(ClaimTypes.Email, correo),
                new Claim(ClaimTypes.Name, nombre),
                new Claim(ClaimTypes.Role, rol)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    ObtenerDuracionToken()
                ),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private int ObtenerDuracionToken()
        {
            return int.TryParse(
                _configuration["JwtSettings:ExpirationMinutes"],
                out int minutos
            ) && minutos > 0
                ? minutos
                : 60;
        }

        private static string? ObtenerRol(int idTipoUsuario)
        {
            return idTipoUsuario switch
            {
                1 => "Administrador",
                2 => "Supervisor",
                3 => "Tecnico",
                4 => "Solicitante",
                _ => null
            };
        }
    }
}
