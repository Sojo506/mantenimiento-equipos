using DA;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace API.Tests.Infraestructura;

public class FabricaApiPruebas : WebApplicationFactory<Program>
{
    private readonly string _nombreBase = $"MantenimientoPruebas-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting("ReglasNegocio:ProcesoAutomaticoHabilitado", "false");
        builder.UseSetting("ReglasNegocio:IdentificacionUsuarioSistema", "USR-001");
        builder.UseSetting("ReglasNegocio:MaxHorasCriticaSinAtender", "4");

        builder.ConfigureServices(services =>
        {
            var registrosDbContext = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<MantenimientoDbContext>) ||
                            d.ServiceType == typeof(DbContextOptions) ||
                            d.ServiceType == typeof(IDbContextOptionsConfiguration<MantenimientoDbContext>))
                .ToList();
            foreach (var registro in registrosDbContext)
                services.Remove(registro);

            services.AddDbContext<MantenimientoDbContext>(options =>
            {
                options.UseInMemoryDatabase(_nombreBase);
                options.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                options.AddInterceptors(new SimuladorRowVersionInterceptor());
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<MantenimientoDbContext>().Database.EnsureCreated();

        return host;
    }

    public async Task EjecutarEnBaseAsync(Func<MantenimientoDbContext, Task> accion)
    {
        using var scope = Services.CreateScope();
        var contexto = scope.ServiceProvider.GetRequiredService<MantenimientoDbContext>();
        await accion(contexto);
    }
}

internal sealed class SimuladorRowVersionInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AsignarVersiones(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        AsignarVersiones(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void AsignarVersiones(DbContext? contexto)
    {
        if (contexto == null) return;

        foreach (var entrada in contexto.ChangeTracker.Entries())
        {
            if (entrada.State is not (EntityState.Added or EntityState.Modified))
                continue;

            var propiedad = entrada.Metadata.FindProperty("RowVersion");
            if (propiedad == null || propiedad.ClrType != typeof(byte[]))
                continue;

            entrada.Property("RowVersion").CurrentValue = Guid.NewGuid().ToByteArray();
        }
    }
}
