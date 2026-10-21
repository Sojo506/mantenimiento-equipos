using Abstracciones.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Configuraciones;

public class EstadoConfiguracion : IEntityTypeConfiguration<Estado>
{
    public void Configure(EntityTypeBuilder<Estado> builder)
    {
        builder.ToTable("ESTADO_TB");

        builder.HasKey(e => e.IdEstado);
        builder.Property(e => e.IdEstado)
            .HasColumnName("ID_ESTADO")
            .ValueGeneratedNever();

        builder.Property(e => e.Nombre)
            .HasColumnName("NOMBRE")
            .HasMaxLength(50)
            .IsRequired();
    }
}
