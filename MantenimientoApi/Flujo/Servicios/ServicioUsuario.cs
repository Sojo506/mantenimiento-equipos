using Abstracciones.DTOs.Usuarios;
using Abstracciones.Entidades;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;

namespace Flujo.Servicios
{
    public class ServicioUsuario : IServicioUsuario
    {
        private readonly IRepositorioUsuario _repositorio;
        private readonly IServicioPasswordHasher _hasher;

        public ServicioUsuario(IRepositorioUsuario repositorio, IServicioPasswordHasher hasher)
        {
            _repositorio = repositorio;
            _hasher = hasher;
        }

        public async Task<IEnumerable<UsuarioDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
        {
            var usuarios = await _repositorio.ObtenerTodosAsync(cancellationToken);
            return usuarios.Select(MapearAUsuarioDto);
        }

        public async Task<UsuarioDto?> ObtenerPorIdentificacionAsync(string identificacion, CancellationToken cancellationToken = default)
        {
            var usuario = await _repositorio.ObtenerPorIdentificacionAsync(identificacion, cancellationToken);
            return usuario == null ? null : MapearAUsuarioDto(usuario);
        }

        public async Task<UsuarioDto> CrearAsync(CrearUsuarioDto dto, CancellationToken cancellationToken = default)
        {
            // Validar duplicados
            if (await _repositorio.ObtenerPorCorreoAsync(dto.Correo, cancellationToken) != null)
                throw new ReglaNegocioException("El correo ya está registrado.");

            if (await _repositorio.ObtenerPorIdentificacionAsync(dto.Identificacion, cancellationToken) != null)
                throw new ReglaNegocioException("La identificación ya está registrada.");

            var nuevoUsuario = new Usuario
            {
                Identificacion = dto.Identificacion,
                Nombre = dto.Nombre,
                PrimerApellido = dto.PrimerApellido,
                SegundoApellido = dto.SegundoApellido,
                Correo = dto.Correo,
                PasswordHash = _hasher.HashearPassword(dto.Password),
                IdTipoUsuario = dto.IdTipoUsuario,
                IdEspecialidad = dto.IdEspecialidad,
                IdEstado = dto.IdEstado,
                Disponibilidad = true
            };

            await _repositorio.AgregarAsync(nuevoUsuario, cancellationToken);
            await _repositorio.GuardarCambiosAsync(cancellationToken);

            // Volvemos a consultar para traer los datos relacionados (TipoUsuario, Estado)
            var usuarioCreado = await _repositorio.ObtenerPorIdentificacionAsync(nuevoUsuario.Identificacion, cancellationToken);
            return MapearAUsuarioDto(usuarioCreado!);
        }


        public async Task ActualizarAsync(
            string identificacion,
            ActualizarUsuarioDto dto,
            CancellationToken cancellationToken = default)
        {
            var usuario = await _repositorio.ObtenerPorIdentificacionAsync(
                identificacion, cancellationToken);

            if (usuario == null)
                throw new NoEncontradoException("Usuario", identificacion);

            // Verificar que el correo no pertenezca a otro usuario
            var usuarioConCorreo = await _repositorio.ObtenerPorCorreoAsync(
                dto.Correo, cancellationToken);

            if (usuarioConCorreo != null &&
                usuarioConCorreo.Identificacion != identificacion)
            {
                throw new ReglaNegocioException(
                    "El correo ya está registrado por otro usuario.");
            }

            // Actualizar información
            usuario.Nombre = dto.Nombre;
            usuario.PrimerApellido = dto.PrimerApellido;
            usuario.SegundoApellido = dto.SegundoApellido;
            usuario.Correo = dto.Correo;
            usuario.IdTipoUsuario = dto.IdTipoUsuario;
            usuario.IdEspecialidad = dto.IdEspecialidad;
            usuario.IdEstado = dto.IdEstado;
            usuario.Disponibilidad = dto.Disponibilidad;

            await _repositorio.ActualizarAsync(usuario, cancellationToken);
            await _repositorio.GuardarCambiosAsync(cancellationToken);
        }

        public async Task EliminarAsync(
            string identificacion,
            CancellationToken cancellationToken = default)
        {
            var usuario = await _repositorio.ObtenerPorIdentificacionAsync(
                identificacion, cancellationToken);

            if (usuario == null)
                throw new NoEncontradoException("Usuario", identificacion);

            usuario.IdEstado = 2;
            usuario.Disponibilidad = false;

            await _repositorio.ActualizarAsync(usuario, cancellationToken);
            await _repositorio.GuardarCambiosAsync(cancellationToken);
        }

        private static UsuarioDto MapearAUsuarioDto(Usuario u)
        {
            return new UsuarioDto
            {
                Identificacion = u.Identificacion,
                Nombre = u.Nombre,
                PrimerApellido = u.PrimerApellido,
                SegundoApellido = u.SegundoApellido,
                Correo = u.Correo,
                IdTipoUsuario = u.IdTipoUsuario,
                RolNombre = u.TipoUsuario?.Nombre ?? "Desconocido",
                IdEspecialidad = u.IdEspecialidad,
                EspecialidadNombre = u.Especialidad?.Nombre,
                Disponibilidad = u.Disponibilidad,
                IdEstado = u.IdEstado,
                EstadoNombre = u.Estado?.Nombre ?? "Desconocido"
            };
        }
    }
}