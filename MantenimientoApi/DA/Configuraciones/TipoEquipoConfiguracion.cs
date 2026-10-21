using Abstracciones.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Configuraciones;

public class TipoEquipoConfiguracion : IEntityTypeConfiguration<TipoEquipo>
{
    public void Configure(EntityTypeBuilder<TipoEquipo> builder)
    {
        builder.ToTable("TIPO_EQUIPO_TB");

        builder.HasKey(t => t.IdTipoEquipo);
        builder.Property(t => t.IdTipoEquipo)
            .HasColumnName("ID_TIPO_EQUIPO")
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
