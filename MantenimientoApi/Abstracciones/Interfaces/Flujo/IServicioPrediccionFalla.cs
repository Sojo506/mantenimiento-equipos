using Abstracciones.DTOs.Equipos;

namespace Abstracciones.Interfaces.Flujo;

/// <summary>
/// Contrato del servicio de aprendizaje automático (ML.NET) para predecir la probabilidad de falla de un equipo a 30 días.
/// </summary>
public interface IServicioPrediccionFalla
{
    Task<PrediccionFallaEquipoDto> PredecirFallaEquipoAsync(string codigoEquipo, CancellationToken cancellationToken = default);
}
