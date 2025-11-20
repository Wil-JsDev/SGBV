using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SGBV.Domain.Common;
using SGBV.Domain.Models;

namespace SGBV.Infrastructure.Persistence.Context;

public class SgbvContext(DbContextOptions<SgbvContext> options) : DbContext(options)
{
    #region DbSets

    public DbSet<Role> Roles { get; set; } 

    public DbSet<User> Users { get; set; } 

    public DbSet<Resource> Resources { get; set; }

    public DbSet<Loan> Loans { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = Guid.NewGuid(),
                NameRol = nameof(Domain.Enum.Roles.Admin),
                CreatedOnUtc = DateTime.UtcNow
            },
            new Role
            {
                Id = Guid.NewGuid(),
                NameRol = nameof(Domain.Enum.Roles.User),
                CreatedOnUtc = DateTime.UtcNow
            }
        );

        #region Filter

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "p");

                var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));

                var body = Expression.Not(property);

                var lambda = Expression.Lambda(body, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        #endregion

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.NameRol).IsRequired().HasMaxLength(30);
            entity.HasIndex(r => r.NameRol).IsUnique();

            entity.Property(e => e.CreatedOnUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(r => r.ProfileUrl).IsRequired(false).HasMaxLength(255);
            entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);

            entity.Property(u => u.RegistrationDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(u => u.LoginAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(u => u.Rol)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.CreatedOnUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.ToTable("Resource");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Title).IsRequired().HasMaxLength(255);
            entity.Property(r => r.Author).IsRequired().HasMaxLength(150);
            entity.Property(r => r.Genre).HasMaxLength(50);
            entity.Property(r => r.CoverUrl).IsRequired(false).HasMaxLength(255);

            entity.Property(r => r.Status).IsRequired();
            entity.Property(r => r.Status).IsRequired().HasDefaultValue(ResourcesStatus.Available);

            entity.Property(r => r.ResourceStatus).IsRequired();
            entity.Property(r => r.ResourceStatus).IsRequired().HasDefaultValue(ResourcesStatus.Available);

            entity.Property(e => e.CreatedOnUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.ToTable("Loan");
            entity.HasKey(l => l.Id);

            entity.Property(l => l.LoanDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(l => l.Status).IsRequired();

            entity.HasOne(l => l.User)
                .WithMany(u => u.Loans)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(l => l.Resource)
                .WithMany(r => r.Loans)
                .HasForeignKey(l => l.ResourceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.CreatedOnUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshToken");
            entity.HasKey(rt => rt.Id);

            entity.Property(rt => rt.Token).IsRequired().HasMaxLength(500);
            entity.HasIndex(rt => rt.Token).IsUnique();

            entity.Property(rt => rt.Used).IsRequired().HasDefaultValue(false);
            entity.Property(rt => rt.Revoked).IsRequired().HasDefaultValue(false);
            entity.Property(rt => rt.Expiration).IsRequired();

            entity.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.CreatedOnUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });
    }
}