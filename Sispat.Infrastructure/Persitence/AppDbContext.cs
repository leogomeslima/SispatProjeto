using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sispat.Domain.Entities;
using System.Reflection;

namespace Sispat.Infrastructure.Persitence
{
    // Herda de IdentityDbContext para incluir as tabelas do Identity
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Nossos DbSets customizados
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

           // Define que SerialNumber (N° de Série) deve ser único
            builder.Entity<Asset>()
                .HasIndex(a => a.SerialNumber)
                .IsUnique();

            // Configura a relação de Asset -> Category
            builder.Entity<Asset>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Assets)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); 
            // Configura a relação de Asset -> Location (Opcional)
            builder.Entity<Asset>()
                .HasOne(a => a.Location)
                .WithMany(l => l.Assets)
                .HasForeignKey(a => a.LocationId)
                .OnDelete(DeleteBehavior.SetNull); 

            // Configura a relação de Asset -> ApplicationUser (Opcional)
            builder.Entity<Asset>()
                .HasOne(a => a.AssignedToUser)
                .WithMany(u => u.Assets)
                .HasForeignKey(a => a.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull); 
        }
    }
}