using Abstracciones.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Configuraciones;

public class RepuestoConfiguracion : IEntityTypeConfiguration<Repuesto>
{
    public void Configure(EntityTypeBuilder<Repuesto> builder)
    {
        builder.ToTable("REPUESTO_TB");

        builder.HasKey(r => r.CodigoRepuesto);
        builder.Property(r => r.CodigoRepuesto)
            .HasColumnName("CODIGO_REPUESTO")
            .HasMaxLength(50);

        builder.Property(r => r.Nombre)
            .HasColumnName("NOMBRE")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Stock)
            .HasColumnName("STOCK")
            .IsRequired();

        builder.Property(r => r.StockMinimo)
            .HasColumnName("STOCK_MINIMO")
            .IsRequired();

        builder.Property(r => r.CostoUnitario)
            .HasColumnName("COSTO_UNITARIO")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(r => r.IdEstado)
            .HasColumnName("ID_ESTADO");

        // Token de concurrencia optimista
        builder.Property(r => r.RowVersion)
            .HasColumnName("ROW_VERSION")
            .IsRowVersion();

        builder.HasOne(r => r.Estado)
            .WithMany()
            .HasForeignKey(r => r.IdEstado)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
