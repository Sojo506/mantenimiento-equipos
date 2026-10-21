using Abstracciones.Interfaces.DA;
using DA.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DA;

/// <summary>
/// Métodos de extensión para registrar los servicios y repositorios de la capa de acceso a datos (DA).
/// </summary>
public static class RegistroDependenciasDA
{
    public static IServiceCollection AgregarCapaDatos(this IServiceCollection services, IConfiguration configuration)
    {
        // Ciclo de vida Scoped: DbContext debe vivir por solicitud HTTP para compartir el seguimiento de entidades y la transacción.
        services.AddDbContext<MantenimientoDbContext>(options =>
        {
            string? connectionString = configuration.GetConnectionString("Default");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                // Fallback de desarrollo para cuando no se haya configurado el secreto aún
                connectionString = "Server=(localdb)\\mssqllocaldb;Database=MantenimientoEquiposDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
            }
            options.UseSqlServer(connectionString);
            options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        });

        // Repositorios con ciclo de vida Scoped: se atan al ciclo de vida del DbContext dentro de cada solicitud HTTP.
        services.AddScoped<IRepositorioEquipo, RepositorioEquipo>();
        services.AddScoped<IRepositorioRepuesto, RepositorioRepuesto>();
        services.AddScoped<IRepositorioOrdenDeTrabajo, RepositorioOrdenDeTrabajo>();
        services.AddScoped<IRepositorioUsuario, RepositorioUsuario>();
        services.AddScoped<IRepositorioPlanMantenimiento, RepositorioPlanMantenimiento>();
        services.AddScoped<IRepositorioFallaRegistrada, RepositorioFallaRegistrada>();
        services.AddScoped<IRepositorioBitacoraOrden, RepositorioBitacoraOrden>();
        services.AddScoped<IRepositorioReportes, RepositorioReportes>();
        services.AddScoped<IRepositorioCatalogos, RepositorioCatalogos>();

        return services;
    }
}
