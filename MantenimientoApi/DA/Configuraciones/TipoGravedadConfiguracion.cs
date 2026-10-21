using Abstracciones.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Configuraciones;

public class TipoGravedadConfiguracion : IEntityTypeConfiguration<TipoGravedad>
{
    public void Configure(EntityTypeBuilder<TipoGravedad> builder)
    {
        builder.ToTable("TIPO_GRAVEDAD_TB");

        builder.HasKey(t => t.IdTipoGravedad);
        builder.Property(t => t.IdTipoGravedad)
            .HasColumnName("ID_TIPO_GRAVEDAD")
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
