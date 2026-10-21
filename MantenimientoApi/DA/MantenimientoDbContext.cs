using Abstracciones.DTOs.Reportes;
using Abstracciones.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DA;

/// <summary>
/// Contexto principal de Entity Framework Core para la solución de mantenimiento.
/// Única clase con acceso directo a la base de datos según la arquitectura del sistema.
/// </summary>
public class MantenimientoDbContext : DbContext
{
    public MantenimientoDbContext(DbContextOptions<MantenimientoDbContext> options) : base(options)
    {
    }

    public DbSet<Estado> Estados => Set<Estado>();
    public DbSet<TipoUsuario> TiposUsuario => Set<TipoUsuario>();
    public DbSet<Especialidad> Especialidades => Set<Especialidad>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<TipoEquipo> TiposEquipo => Set<TipoEquipo>();
    public DbSet<Marca> Marcas => Set<Marca>();
    public DbSet<Modelo> Modelos => Set<Modelo>();
    public DbSet<Ubicacion> Ubicaciones => Set<Ubicacion>();
    public DbSet<Equipo> Equipos => Set<Equipo>();
    public DbSet<TipoOrden> TiposOrden => Set<TipoOrden>();
    public DbSet<TipoGravedad> TiposGravedad => Set<TipoGravedad>();
    public DbSet<Repuesto> Repuestos => Set<Repuesto>();
    public DbSet<OrdenDeTrabajo> OrdenesDeTrabajo => Set<OrdenDeTrabajo>();
    public DbSet<OrdenRepuesto> OrdenesRepuestos => Set<OrdenRepuesto>();
    public DbSet<FallaRegistrada> FallasRegistradas => Set<FallaRegistrada>();
    public DbSet<PlanDeMantenimiento> PlanesDeMantenimiento => Set<PlanDeMantenimiento>();
    public DbSet<BitacoraOrden> BitacorasOrden => Set<BitacoraOrden>();

    // Conjunto para el resultado del Stored Procedure sp_ReporteConsumoRepuestosPorPeriodo
    public DbSet<ReporteConsumoRepuestoDto> ReporteConsumoRepuestos => Set<ReporteConsumoRepuestoDto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica todas las configuraciones Fluent API de la carpeta Configuraciones
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MantenimientoDbContext).Assembly);

        // Mapeo sin clave primaria para el resultado del procedimiento almacenado
        modelBuilder.Entity<ReporteConsumoRepuestoDto>(entity =>
        {
            entity.HasNoKey();
            entity.ToView(null);
            entity.Property(r => r.CostoTotalAcumulado).HasColumnType("decimal(18,2)");
        });

        // Configuración de los datos semilla (Seed Data)
        DA.Semilla.MantenimientoSeedData.ConfigurarSemilla(modelBuilder);
    }
}
