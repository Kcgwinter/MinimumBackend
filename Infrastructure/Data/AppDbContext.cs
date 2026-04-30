using Core.Entities;
using Infrastructure.DBSets;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            modelBuilder
                .Entity<Permission>()
                .HasData(
                    new Permission { Id = 1, Name = "CreateUser" },
                    new Permission { Id = 2, Name = "EditUser" },
                    new Permission { Id = 3, Name = "DeleteUser" },
                    new Permission { Id = 4, Name = "ViewUser" }
                );

            modelBuilder
                .Entity<Role>()
                .HasData(new Role { Id = 1, Name = "Admin" }, new Role { Id = 2, Name = "User" });
        }

        // Override SaveChangesAsync to automatically set CreatedAt and UpdatedAt timestamps and handle soft delete logic
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
