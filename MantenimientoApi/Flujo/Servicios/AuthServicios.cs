using Abstracciones.DTOs.Usuarios;
using Abstracciones.Entidades;
using Abstracciones.Interfaces; // <-- Asegúrate de tener este using para IAuthServicio e IServicioPasswordHasher
using Abstracciones.Interfaces.DA; // <-- Tu namespace para IRepositorioUsuario
using Abstracciones.Interfaces.Flujo;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Flujo.Servicios
{
    // 1. Se implementa la interfaz IAuthServicio
    public class AuthServicio : IAuthServicio
    {
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IConfiguration _configuracion;
        private readonly IServicioPasswordHasher _passwordHasher; // 2. Se inyecta tu Hasher

        public AuthServicio(
            IRepositorioUsuario repositorioUsuario,
            IConfiguration configuracion,
            IServicioPasswordHasher passwordHasher) // 3. Se agrega al constructor
        {
            _repositorioUsuario = repositorioUsuario;
            _configuracion = configuracion;
            _passwordHasher = passwordHasher;
        }

        public async Task<RespuestaLoginDto?> LoginAsync(LoginDto loginDto)
        {
            // 1. Buscar al usuario por correo
            var usuario = await _repositorioUsuario.ObtenerPorCorreoAsync(loginDto.Correo);

            // 2. Validar que el usuario exista
            if (usuario == null)
                return null;

            // 3. Validar que el usuario esté activo 
            if (usuario.IdEstado != 1 || !usuario.Disponibilidad)
            {
                throw new UnauthorizedAccessException(
                    "El usuario se encuentra inactivo o no disponible.");
            }

            // 4. Verificar la contraseña usando TU SERVICIO (Adiós BCrypt directo)
            bool passwordValida = _passwordHasher.VerificarPassword(loginDto.Password, usuario.PasswordHash);

            if (!passwordValida)
                return null;

            // 5. Generar el Token
            string token = GenerarToken(usuario);

            // 6. Mapear la entidad al DTO de salida
            var usuarioDto = new UsuarioDto
            {
                Identificacion = usuario.Identificacion,
                Nombre = usuario.Nombre,
                PrimerApellido = usuario.PrimerApellido,
                SegundoApellido = usuario.SegundoApellido,
                Correo = usuario.Correo,
                IdTipoUsuario = usuario.IdTipoUsuario,
                RolNombre = usuario.TipoUsuario?.Nombre ?? "Desconocido",
                IdEspecialidad = usuario.IdEspecialidad,
                EspecialidadNombre = usuario.Especialidad?.Nombre,
                Disponibilidad = usuario.Disponibilidad,
                IdEstado = usuario.IdEstado,
                EstadoNombre = usuario.Estado?.Nombre ?? "Desconocido"
            };

            // 7. Retornar la respuesta
            return new RespuestaLoginDto
            {
                Token = token,
                Expiracion = DateTime.UtcNow.AddHours(2),
                Usuario = usuarioDto
            };
        }

        public string GenerarToken(Usuario usuario)
        {
            var jwtSettings = _configuracion.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]!;
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            // Claims (Información del usuario dentro del token)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Correo),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, usuario.Identificacion),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.PrimerApellido}"),
                new Claim(ClaimTypes.Role, usuario.TipoUsuario?.Nombre ?? "Solicitante")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}