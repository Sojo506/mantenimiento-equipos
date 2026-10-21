using Abstracciones.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Configuraciones;

public class FallaRegistradaConfiguracion : IEntityTypeConfiguration<FallaRegistrada>
{
    public void Configure(EntityTypeBuilder<FallaRegistrada> builder)
    {
        builder.ToTable("FALLA_REGISTRADA_TB");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id)
            .HasColumnName("ID_FALLA")
            .ValueGeneratedOnAdd();

        builder.Property(f => f.CodigoEquipo)
            .HasColumnName("CODIGO_EQUIPO")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(f => f.Fecha)
            .HasColumnName("FECHA")
            .HasColumnType("datetime2");

        builder.Property(f => f.IdTipoGravedad)
            .HasColumnName("ID_TIPO_GRAVEDAD");

        builder.Property(f => f.Descripcion)
            .HasColumnName("DESCRIPCION")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(f => f.NumeroOrden)
            .HasColumnName("ORDEN_ASOCIADA");

        // Relaciones
        builder.HasOne(f => f.Equipo)
            .WithMany(e => e.FallasRegistradas)
            .HasForeignKey(f => f.CodigoEquipo)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.TipoGravedad)
            .WithMany(t => t.FallasRegistradas)
            .HasForeignKey(f => f.IdTipoGravedad)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.OrdenDeTrabajo)
            .WithMany(o => o.FallasRegistradas)
            .HasForeignKey(f => f.NumeroOrden)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
