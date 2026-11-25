using Abstracciones.DTOs.Fallas;

namespace Abstracciones.Interfaces.Flujo;

/// <summary>
/// Contrato de la lógica de negocio para el registro y consulta de fallas de equipos,
/// incluyendo la transición automática a estado EnRiesgo (RN-03).
/// </summary>
public interface IServicioFalla
{
    Task<IEnumerable<FallaRegistradaDto>> ObtenerTodasAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<FallaRegistradaDto>> ObtenerPorEquipoAsync(string codigoEquipo, CancellationToken cancellationToken = default);
    Task<FallaRegistradaDto> ObtenerPorIdAsync(int idFalla, CancellationToken cancellationToken = default);
    Task<FallaRegistradaDto> RegistrarAsync(RegistrarFallaDto dto, CancellationToken cancellationToken = default);
}
