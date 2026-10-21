using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DA;

/// <summary>
/// Fábrica en tiempo de diseño para que las herramientas de EF Core (dotnet ef)
/// puedan construir el modelo y generar migraciones sin requerir conexión en vivo.
/// </summary>
public class MantenimientoDbContextFactory : IDesignTimeDbContextFactory<MantenimientoDbContext>
{
    public MantenimientoDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MantenimientoDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MantenimientoEquiposDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
        optionsBuilder.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));

        return new MantenimientoDbContext(optionsBuilder.Options);
    }
}
