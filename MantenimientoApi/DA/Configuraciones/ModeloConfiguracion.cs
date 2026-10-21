using Abstracciones.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Configuraciones;

public class ModeloConfiguracion : IEntityTypeConfiguration<Modelo>
{
    public void Configure(EntityTypeBuilder<Modelo> builder)
    {
        builder.ToTable("MODELO_TB");

        builder.HasKey(m => m.IdModelo);
        builder.Property(m => m.IdModelo)
            .HasColumnName("ID_MODELO")
            .ValueGeneratedNever();

        builder.Property(m => m.Nombre)
            .HasColumnName("NOMBRE")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(m => m.IdEstado)
            .HasColumnName("ID_ESTADO");

        builder.HasOne(m => m.Estado)
            .WithMany()
            .HasForeignKey(m => m.IdEstado)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
