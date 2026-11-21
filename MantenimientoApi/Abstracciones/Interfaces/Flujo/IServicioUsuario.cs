using Abstracciones.DTOs.Usuarios;

namespace Abstracciones.Interfaces.Flujo
{
    public interface IServicioUsuario
    {

        Task<IEnumerable<UsuarioDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
        Task<UsuarioDto?> ObtenerPorIdentificacionAsync(string identificacion, CancellationToken cancellationToken = default);
        Task<UsuarioDto> CrearAsync(CrearUsuarioDto dto, CancellationToken cancellationToken = default);
        Task ActualizarAsync(string identificacion, ActualizarUsuarioDto dto, CancellationToken cancellationToken = default);
        Task EliminarAsync(string identificacion, CancellationToken cancellationToken = default);
    }
}
