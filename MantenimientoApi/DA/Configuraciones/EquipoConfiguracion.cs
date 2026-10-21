using Abstracciones.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Configuraciones;

public class EquipoConfiguracion : IEntityTypeConfiguration<Equipo>
{
    public void Configure(EntityTypeBuilder<Equipo> builder)
    {
        builder.ToTable("EQUIPO_TB");

        builder.HasKey(e => e.CodigoEquipo);
        builder.Property(e => e.CodigoEquipo)
            .HasColumnName("CODIGO_EQUIPO")
            .HasMaxLength(50);

        builder.Property(e => e.Nombre)
            .HasColumnName("NOMBRE")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.IdTipoEquipo)
            .HasColumnName("ID_TIPO_EQUIPO");

        builder.Property(e => e.IdMarca)
            .HasColumnName("ID_MARCA");

        builder.Property(e => e.IdModelo)
            .HasColumnName("ID_MODELO");

        builder.Property(e => e.IdUbicacion)
            .HasColumnName("ID_UBICACION");

        builder.Property(e => e.FechaAdquisicion)
            .HasColumnName("FECHA_ADQUISICION")
            .HasColumnType("datetime2");

        builder.Property(e => e.IdEstado)
            .HasColumnName("ID_ESTADO");

        // Relaciones
        builder.HasOne(e => e.TipoEquipo)
            .WithMany(t => t.Equipos)
            .HasForeignKey(e => e.IdTipoEquipo)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Marca)
            .WithMany(m => m.Equipos)
            .HasForeignKey(e => e.IdMarca)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Modelo)
            .WithMany(m => m.Equipos)
            .HasForeignKey(e => e.IdModelo)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Ubicacion)
            .WithMany(u => u.Equipos)
            .HasForeignKey(e => e.IdUbicacion)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Estado)
            .WithMany()
            .HasForeignKey(e => e.IdEstado)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
