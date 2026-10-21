using Abstracciones.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Configuraciones;

public class UsuarioConfiguracion : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("USUARIO_TB");

        builder.HasKey(u => u.Identificacion);
        builder.Property(u => u.Identificacion)
            .HasColumnName("IDENTIFICACION")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Nombre)
            .HasColumnName("NOMBRE")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.PrimerApellido)
            .HasColumnName("PRIMER_APELLIDO")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.SegundoApellido)
            .HasColumnName("SEGUNDO_APELLIDO")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Correo)
            .HasColumnName("CORREO")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(u => u.Correo)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("PASSWORD_HASH")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.IdTipoUsuario)
            .HasColumnName("ID_TIPO_USUARIO");

        builder.Property(u => u.IdEspecialidad)
            .HasColumnName("ID_ESPECIALIDAD")
            .IsRequired(false);

        builder.Property(u => u.Disponibilidad)
            .HasColumnName("DISPONIBILIDAD")
            .HasDefaultValue(true);

        builder.Property(u => u.IdEstado)
            .HasColumnName("ID_ESTADO");

        // Relaciones
        builder.HasOne(u => u.TipoUsuario)
            .WithMany(t => t.Usuarios)
            .HasForeignKey(u => u.IdTipoUsuario)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Especialidad)
            .WithMany(e => e.Usuarios)
            .HasForeignKey(u => u.IdEspecialidad)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Estado)
            .WithMany()
            .HasForeignKey(u => u.IdEstado)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
