using Abstracciones.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Configuraciones;

public class PlanDeMantenimientoConfiguracion : IEntityTypeConfiguration<PlanDeMantenimiento>
{
    public void Configure(EntityTypeBuilder<PlanDeMantenimiento> builder)
    {
        builder.ToTable("PLAN_MANTENIMIENTO_TB");

        builder.HasKey(p => p.IdPlanMantenimiento);
        builder.Property(p => p.IdPlanMantenimiento)
            .HasColumnName("ID_PLAN_MANTENIMIENTO")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.CodigoEquipo)
            .HasColumnName("CODIGO_EQUIPO")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Frecuencia)
            .HasColumnName("FRECUENCIA")
            .IsRequired();

        builder.Property(p => p.ProximaFecha)
            .HasColumnName("PROXIMA_FECHA")
            .HasColumnType("datetime2");

        builder.Property(p => p.IdEstado)
            .HasColumnName("ID_ESTADO");

        builder.HasOne(p => p.Equipo)
            .WithMany(e => e.PlanesDeMantenimiento)
            .HasForeignKey(p => p.CodigoEquipo)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Estado)
            .WithMany()
            .HasForeignKey(p => p.IdEstado)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
