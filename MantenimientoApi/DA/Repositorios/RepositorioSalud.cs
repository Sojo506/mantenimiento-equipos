using Abstracciones.Interfaces.DA;
using Microsoft.EntityFrameworkCore;

namespace DA.Repositorios;

public class RepositorioSalud : IRepositorioSalud
{
    private readonly MantenimientoDbContext _context;

    public RepositorioSalud(MantenimientoDbContext context)
    {
        _context = context;
    }

    public async Task<bool> VerificarConexionAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Database.CanConnectAsync(
            cancellationToken);
    }
}