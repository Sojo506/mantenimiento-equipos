using Abstracciones.DTOs.Equipos;

namespace Abstracciones.Interfaces.Flujo;

/// <summary>
/// Contrato de la lógica de negocio para la gestión de equipos.
/// </summary>
public interface IServicioEquipo
{
    Task<IEnumerable<EquipoDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<EquipoDto> ObtenerPorCodigoAsync(string codigoEquipo, CancellationToken cancellationToken = default);
    Task<EquipoDto> CrearAsync(CrearEquipoDto dto, CancellationToken cancellationToken = default);
    Task<EquipoDto> ActualizarAsync(string codigoEquipo, ActualizarEquipoDto dto, CancellationToken cancellationToken = default);
    Task EliminarAsync(string codigoEquipo, CancellationToken cancellationToken = default);
}
