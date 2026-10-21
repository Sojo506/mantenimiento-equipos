using Abstracciones.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Configuraciones;

public class OrdenDeTrabajoConfiguracion : IEntityTypeConfiguration<OrdenDeTrabajo>
{
    public void Configure(EntityTypeBuilder<OrdenDeTrabajo> builder)
    {
        builder.ToTable("ORDEN_TB");

        builder.HasKey(o => o.Numero);
        builder.Property(o => o.Numero)
            .HasColumnName("NUMERO")
            .ValueGeneratedOnAdd();

        builder.Property(o => o.CodigoEquipo)
            .HasColumnName("ID_EQUIPO")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.IdentificacionSolicitante)
            .HasColumnName("IDENTIFICACION_SOLICITANTE")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.IdentificacionTecnico)
            .HasColumnName("IDENTIFICACION_TECNICO")
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(o => o.IdTipoOrden)
            .HasColumnName("ID_TIPO_ORDEN");

        builder.Property(o => o.Prioridad)
            .HasColumnName("PRIORIDAD")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(o => o.FechaCreacion)
            .HasColumnName("FECHA_CREACION")
            .HasColumnType("datetime2");

        builder.Property(o => o.FechaLimite)
            .HasColumnName("FECHA_LIMITE")
            .HasColumnType("datetime2");

        builder.Property(o => o.TiempoInvertido)
            .HasColumnName("TIEMPO_INVERTIDO")
            .HasColumnType("decimal(5,2)")
            .HasDefaultValue(0);

        builder.Property(o => o.Descripcion)
            .HasColumnName("DESCRIPCION")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(o => o.SinRepuestos)
            .HasColumnName("SIN_REPUESTOS")
            .HasDefaultValue(false);

        builder.Property(o => o.JustificacionSinRepuestos)
            .HasColumnName("JUSTIFICACION_SIN_REPUESTOS")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(o => o.IdEstado)
            .HasColumnName("ID_ESTADO");

        // Token de concurrencia optimista
        builder.Property(o => o.RowVersion)
            .HasColumnName("ROW_VERSION")
            .IsRowVersion();

        // Relaciones
        builder.HasOne(o => o.Equipo)
            .WithMany(e => e.OrdenesDeTrabajo)
            .HasForeignKey(o => o.CodigoEquipo)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Solicitante)
            .WithMany(u => u.OrdenesSolicitadas)
            .HasForeignKey(o => o.IdentificacionSolicitante)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Tecnico)
            .WithMany(u => u.OrdenesAsignadas)
            .HasForeignKey(o => o.IdentificacionTecnico)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.TipoOrden)
            .WithMany(t => t.OrdenesDeTrabajo)
            .HasForeignKey(o => o.IdTipoOrden)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Estado)
            .WithMany()
            .HasForeignKey(o => o.IdEstado)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
