using Abstracciones.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Configuraciones;

public class TipoUsuarioConfiguracion : IEntityTypeConfiguration<TipoUsuario>
{
    public void Configure(EntityTypeBuilder<TipoUsuario> builder)
    {
        builder.ToTable("TIPO_USUARIO_TB");

        builder.HasKey(t => t.IdTipoUsuario);
        builder.Property(t => t.IdTipoUsuario)
            .HasColumnName("ID_TIPO_USUARIO")
            .ValueGeneratedNever();

        builder.Property(t => t.Nombre)
            .HasColumnName("NOMBRE")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.IdEstado)
            .HasColumnName("ID_ESTADO");

        builder.HasOne(t => t.Estado)
            .WithMany()
            .HasForeignKey(t => t.IdEstado)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
