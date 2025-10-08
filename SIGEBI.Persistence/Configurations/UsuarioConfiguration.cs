using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Persistence.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");

            builder.HasKey(u => u.Id);

            // EmailAddress ValueObject conversion (forma strongly-typed)
            var emailConverter = new ValueConverter<EmailAddress, string>(
                v => v.Value,
                v => EmailAddress.Create(v));

            builder.Property(u => u.Email)
                   .HasConversion(emailConverter)
                   .HasColumnName("Email")
                   .HasMaxLength(256)
                   .IsRequired();

            // NombreCompleto (owned)
            builder.OwnsOne(u => u.Nombre, nb =>
            {
                nb.Property(n => n.Nombres)
                  .HasColumnName("Nombres")
                  .HasMaxLength(100)
                  .IsRequired();

                nb.Property(n => n.Apellidos)
                  .HasColumnName("Apellidos")
                  .HasMaxLength(120)
                  .IsRequired();
            });

            builder.Property(u => u.Tipo)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(u => u.Activo).IsRequired();
            builder.Property(u => u.CreatedAtUtc).IsRequired();
            builder.Property(u => u.UpdatedAtUtc);

            builder.HasIndex(u => u.Email).IsUnique();

            // Many-to-Many con Roles usando el backing field "_roles"
            builder.Navigation("_roles")
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany<Rol>("_roles")
                   .WithMany()
                   .UsingEntity<Dictionary<string, object>>(
                        "UsuarioRoles",
                        right => right
                            .HasOne<Rol>()
                            .WithMany()
                            .HasForeignKey("RolId")
                            .OnDelete(DeleteBehavior.Restrict),
                        left => left
                            .HasOne<Usuario>()
                            .WithMany()
                            .HasForeignKey("UsuarioId")
                            .OnDelete(DeleteBehavior.Restrict),
                        join =>
                        {
                            join.ToTable("UsuarioRoles");
                            join.HasKey("UsuarioId", "RolId");
                            join.HasIndex("RolId");
                            join.HasIndex("UsuarioId");
                        });

            // PrestamosIds: no se persiste (backing field interno).
        }
    }
}