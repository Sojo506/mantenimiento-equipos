using Abstracciones.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Configuraciones;

public class OrdenRepuestoConfiguracion : IEntityTypeConfiguration<OrdenRepuesto>
{
    public void Configure(EntityTypeBuilder<OrdenRepuesto> builder)
    {
        builder.ToTable("ORDEN_REPUESTO_TB");

        builder.HasKey(or => new { or.Numero, or.CodigoRepuesto });

        builder.Property(or => or.Numero)
            .HasColumnName("NUMERO");

        builder.Property(or => or.CodigoRepuesto)
            .HasColumnName("CODIGO_REPUESTO")
            .HasMaxLength(50);

        builder.Property(or => or.CantidadUsada)
            .HasColumnName("CANTIDAD_USADA")
            .IsRequired();

        builder.Property(or => or.CostoAplicado)
            .HasColumnName("COSTO_APLICADO")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne(or => or.OrdenDeTrabajo)
            .WithMany(o => o.RepuestosUsados)
            .HasForeignKey(or => or.Numero)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(or => or.Repuesto)
            .WithMany(r => r.OrdenesRepuestos)
            .HasForeignKey(or => or.CodigoRepuesto)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
