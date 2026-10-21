using Abstracciones.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Configuraciones;

public class TipoOrdenConfiguracion : IEntityTypeConfiguration<TipoOrden>
{
    public void Configure(EntityTypeBuilder<TipoOrden> builder)
    {
        builder.ToTable("TIPO_ORDEN_TB");

        builder.HasKey(t => t.IdTipoOrden);
        builder.Property(t => t.IdTipoOrden)
            .HasColumnName("ID_TIPO_ORDEN")
            .ValueGeneratedNever();

        builder.Property(t => t.Nombre)
            .HasColumnName("NOMBRE")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.IdEstado)
            .HasColumnName("ID_ESTADO");

        builder.HasOne(t => t.Estado)
            .WithMany()
            .HasForeignKey(t => t.IdEstado)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
