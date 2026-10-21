using Abstracciones.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Configuraciones;

public class MarcaConfiguracion : IEntityTypeConfiguration<Marca>
{
    public void Configure(EntityTypeBuilder<Marca> builder)
    {
        builder.ToTable("MARCA_TB");

        builder.HasKey(m => m.IdMarca);
        builder.Property(m => m.IdMarca)
            .HasColumnName("ID_MARCA")
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
