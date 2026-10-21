using Abstracciones.Configuracion;
using Abstracciones.Interfaces;
using Abstracciones.Interfaces.Flujo;
using Flujo.Servicios;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flujo;

/// <summary>
/// Métodos de extensión para registrar los servicios de lógica de negocio de la capa Flujo.
/// </summary>
public static class RegistroDependenciasFlujo
{
    public static IServiceCollection AgregarCapaFlujo(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuración fuertemente tipada del patrón Options para umbrales y plazos de reglas
        services.Configure<ReglasNegocioOptions>(configuration.GetSection(ReglasNegocioOptions.Seccion));

        // Servicios de negocio con ciclo de vida Scoped: viven por solicitud HTTP y coordinan con repositorios Scoped.
        services.AddScoped<IServicioEquipo, ServicioEquipo>();
        services.AddScoped<IServicioRepuesto, ServicioRepuesto>();
        services.AddScoped<IServicioOrdenDeTrabajo, ServicioOrdenDeTrabajo>();
        services.AddScoped<IServicioCatalogos, ServicioCatalogos>();

        // Servicio de hashing con ciclo de vida Singleton: sin estado mutable, seguro para concurrencia entre hilos.
        services.AddSingleton<IServicioPasswordHasher, ServicioPasswordHasher>();

        return services;
    }
}
