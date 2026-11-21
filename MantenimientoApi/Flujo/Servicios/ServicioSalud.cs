using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;

namespace Flujo.Servicios;

public class ServicioSalud : IServicioSalud
{
    private readonly IRepositorioSalud _repositorioSalud;

    public ServicioSalud(IRepositorioSalud repositorioSalud)
    {
        _repositorioSalud = repositorioSalud;
    }

    public Task<bool> VerificarConexionAsync(
        CancellationToken cancellationToken = default)
    {
        return _repositorioSalud.VerificarConexionAsync(
            cancellationToken);
    }
}