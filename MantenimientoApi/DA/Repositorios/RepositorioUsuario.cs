using Abstracciones.Entidades;
using Abstracciones.Interfaces.DA;
using Microsoft.EntityFrameworkCore;

namespace DA.Repositorios;

/// <summary>
/// Implementación del repositorio de acceso a datos para la entidad Usuario.
/// </summary>
public class RepositorioUsuario : IRepositorioUsuario
{
    private readonly MantenimientoDbContext _context;

    public RepositorioUsuario(MantenimientoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Usuario>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .Include(u => u.TipoUsuario)
            .Include(u => u.Especialidad)
            .Include(u => u.Estado)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Usuario?> ObtenerPorIdentificacionAsync(string identificacion, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .Include(u => u.TipoUsuario)
            .Include(u => u.Especialidad)
            .Include(u => u.Estado)
            .FirstOrDefaultAsync(u => u.Identificacion == identificacion, cancellationToken);
    }

    public async Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .Include(u => u.TipoUsuario)
            .Include(u => u.Especialidad)
            .Include(u => u.Estado)
            .FirstOrDefaultAsync(u => u.Correo == correo, cancellationToken);
    }

    public async Task<IEnumerable<Usuario>> ObtenerTecnicosDisponiblesAsync(CancellationToken cancellationToken = default)
    {
        // IdTipoUsuario = 3 (Tecnico), Disponibilidad = true, IdEstado = 1 (Activo)
        return await _context.Usuarios
            .Where(u => u.IdTipoUsuario == 3 && u.Disponibilidad && u.IdEstado == 1)
            .Include(u => u.Especialidad)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AgregarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        await _context.Usuarios.AddAsync(usuario, cancellationToken);
    }

    public Task ActualizarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        _context.Usuarios.Update(usuario);
        return Task.CompletedTask;
    }

    public async Task GuardarCambiosAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
