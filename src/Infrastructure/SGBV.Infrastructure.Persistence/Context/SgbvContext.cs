using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SGBV.Domain.Common;
using SGBV.Domain.Models;

namespace SGBV.Infrastructure.Persistence.Context;

public class SgbvContext(DbContextOptions<SgbvContext> options) : DbContext(options)
{
    #region DbSets

    public DbSet<Role> Roles { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Resource> Resources { get; set; } = null!;

    public DbSet<Loan> Loans { get; set; } = null!;

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region Filter

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Verifica si la entidad hereda de BaseEntity
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "p");

                // Accede a la propiedad IsDeleted
                var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));

                var body = Expression.Not(property);

                var lambda = Expression.Lambda(body, parameter);

                // Se aplica el filtro
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        #endregion

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(r => r.RolId);
            entity.Property(r => r.NameRol).IsRequired().HasMaxLength(30);
            entity.HasIndex(r => r.NameRol).IsUnique();

            entity.Property(e => e.CreatedOnUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
            entity.HasKey(u => u.UserId);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);

            entity.Property(u => u.RegistrationDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(u => u.LoginAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relaciones
            entity.HasOne(u => u.Rol)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de BaseEntity
            entity.Property(e => e.CreatedOnUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.ToTable("Resource");
            entity.HasKey(r => r.ResourceId);
            entity.Property(r => r.Title).IsRequired().HasMaxLength(255);
            entity.Property(r => r.Author).IsRequired().HasMaxLength(150);
            entity.Property(r => r.Genre).HasMaxLength(50);

            entity.Property(r => r.Status).IsRequired();
            entity.Property(r => r.Status).IsRequired().HasDefaultValue(ResourcesStatus.Available);

            entity.Property(r => r.ResourceStatus).IsRequired();
            entity.Property(r => r.ResourceStatus).IsRequired().HasDefaultValue(ResourcesStatus.Available);

            // Configuración de BaseEntity
            entity.Property(e => e.CreatedOnUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.ToTable("Loan");
            entity.HasKey(l => l.LoanId);

            entity.Property(l => l.LoanDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(l => l.Status).IsRequired();

            // Relaciones
            entity.HasOne(l => l.User)
                .WithMany(u => u.Loans)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(l => l.Resource)
                .WithMany(r => r.Loans)
                .HasForeignKey(l => l.ResourceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de BaseEntity
            entity.Property(e => e.CreatedOnUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });
    }
}