using Abstracciones.DTOs.Usuarios;
using Abstracciones.Entidades;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Flujo.Servicios
{
    public class AuthServicio : IAuthServicio
    {
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IConfiguration _configuracion;
        private readonly IServicioPasswordHasher _passwordHasher;

        public AuthServicio(
            IRepositorioUsuario repositorioUsuario,
            IConfiguration configuracion,
            IServicioPasswordHasher passwordHasher)
        {
            _repositorioUsuario = repositorioUsuario;
            _configuracion = configuracion;
            _passwordHasher = passwordHasher;
        }

        public async Task<RespuestaLoginDto?> LoginAsync(LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Correo) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                throw new ReglaNegocioException("Debe ingresar el correo y la contraseña.");
            }

            var correo = loginDto.Correo.Trim().ToLowerInvariant();
            var usuario = await _repositorioUsuario.ObtenerPorCorreoAsync(correo);

            if (usuario == null)
            {
                return null;
            }

            if (usuario.IdEstado != 1 || !usuario.Disponibilidad)
            {
                throw new NoAutorizadoException("El usuario está inactivo o no disponible.");
            }

            bool passwordValida = _passwordHasher.VerificarPassword(loginDto.Password, usuario.PasswordHash);
            if (!passwordValida)
            {
                return null;
            }

            string rol = usuario.TipoUsuario?.Nombre ?? ObtenerRolPorId(usuario.IdTipoUsuario);
            string token = GenerarToken(usuario);
            int minutos = ObtenerDuracionToken();

            var usuarioDto = new UsuarioDto
            {
                Identificacion = usuario.Identificacion,
                Nombre = usuario.Nombre,
                PrimerApellido = usuario.PrimerApellido,
                SegundoApellido = usuario.SegundoApellido,
                Correo = usuario.Correo,
                IdTipoUsuario = usuario.IdTipoUsuario,
                RolNombre = rol,
                IdEspecialidad = usuario.IdEspecialidad,
                EspecialidadNombre = usuario.Especialidad?.Nombre,
                Disponibilidad = usuario.Disponibilidad,
                IdEstado = usuario.IdEstado,
                EstadoNombre = usuario.Estado?.Nombre ?? "Activo"
            };

            return new RespuestaLoginDto
            {
                Mensaje = "Inicio de sesión exitoso.",
                Token = token,
                TipoToken = "Bearer",
                Expiracion = DateTime.UtcNow.AddMinutes(minutos),
                Usuario = usuarioDto
            };
        }

        public string GenerarToken(Usuario usuario)
        {
            var jwtSettings = _configuracion.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            if (string.IsNullOrWhiteSpace(secretKey) || Encoding.UTF8.GetByteCount(secretKey) < 32)
            {
                throw new InvalidOperationException("La clave JWT debe tener al menos 32 bytes.");
            }

            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            string rol = usuario.TipoUsuario?.Nombre ?? ObtenerRolPorId(usuario.IdTipoUsuario);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Correo),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, usuario.Identificacion),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Role, rol)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(ObtenerDuracionToken()),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private int ObtenerDuracionToken()
        {
            return int.TryParse(_configuracion["JwtSettings:ExpirationMinutes"], out int minutos) && minutos > 0
                ? minutos
                : 60;
        }

        private static string ObtenerRolPorId(int idTipoUsuario) => idTipoUsuario switch
        {
            1 => "Administrador",
            2 => "Supervisor",
            3 => "Tecnico",
            4 => "Solicitante",
            _ => "Solicitante"
        };
    }
}