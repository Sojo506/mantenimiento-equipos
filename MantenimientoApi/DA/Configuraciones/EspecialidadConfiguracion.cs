using Abstracciones.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Configuraciones;

public class EspecialidadConfiguracion : IEntityTypeConfiguration<Especialidad>
{
    public void Configure(EntityTypeBuilder<Especialidad> builder)
    {
        builder.ToTable("ESPECIALIDAD_TB");

        builder.HasKey(e => e.IdEspecialidad);
        builder.Property(e => e.IdEspecialidad)
            .HasColumnName("ID_ESPECIALIDAD")
            .ValueGeneratedNever();

        builder.Property(e => e.Nombre)
            .HasColumnName("NOMBRE")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.IdEstado)
            .HasColumnName("ID_ESTADO");

        builder.HasOne(e => e.Estado)
            .WithMany()
            .HasForeignKey(e => e.IdEstado)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
