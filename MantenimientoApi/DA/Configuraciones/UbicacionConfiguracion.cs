using Abstracciones.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Configuraciones;

public class UbicacionConfiguracion : IEntityTypeConfiguration<Ubicacion>
{
    public void Configure(EntityTypeBuilder<Ubicacion> builder)
    {
        builder.ToTable("UBICACION_TB");

        builder.HasKey(u => u.IdUbicacion);
        builder.Property(u => u.IdUbicacion)
            .HasColumnName("ID_UBICACION")
            .ValueGeneratedNever();

        builder.Property(u => u.Nombre)
            .HasColumnName("NOMBRE")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.IdEstado)
            .HasColumnName("ID_ESTADO");

        builder.HasOne(u => u.Estado)
            .WithMany()
            .HasForeignKey(u => u.IdEstado)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
