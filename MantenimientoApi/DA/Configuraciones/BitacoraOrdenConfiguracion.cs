using Abstracciones.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Configuraciones;

public class BitacoraOrdenConfiguracion : IEntityTypeConfiguration<BitacoraOrden>
{
    public void Configure(EntityTypeBuilder<BitacoraOrden> builder)
    {
        builder.ToTable("BITACORA_ORDEN_TB");

        builder.HasKey(b => b.IdBitacora);
        builder.Property(b => b.IdBitacora)
            .HasColumnName("ID_BITACORA")
            .ValueGeneratedOnAdd();

        builder.Property(b => b.Numero)
            .HasColumnName("ORDEN");

        builder.Property(b => b.Identificacion)
            .HasColumnName("IDENTIFICACION")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(b => b.Fecha)
            .HasColumnName("FECHA")
            .HasColumnType("datetime2");

        builder.Property(b => b.EstadoAnterior)
            .HasColumnName("ESTADO_ANTERIOR")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(b => b.EstadoNuevo)
            .HasColumnName("ESTADO_NUEVO")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(b => b.Comentario)
            .HasColumnName("COMENTARIO")
            .HasMaxLength(500)
            .IsRequired();

        builder.HasOne(b => b.OrdenDeTrabajo)
            .WithMany(o => o.Bitacoras)
            .HasForeignKey(b => b.Numero)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Usuario)
            .WithMany(u => u.Bitacoras)
            .HasForeignKey(b => b.Identificacion)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
