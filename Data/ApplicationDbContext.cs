using Microsoft.EntityFrameworkCore;
using SpacePirates.API.Models;
using SpacePirates.API.Models.ShipComponents;

namespace SpacePirates.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ship> Ships { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Hull> Hulls { get; set; }
        public DbSet<Shield> Shields { get; set; }
        public DbSet<Engine> Engines { get; set; }
        public DbSet<FuelSystem> FuelSystems { get; set; }
        public DbSet<CargoSystem> CargoSystems { get; set; }
        public DbSet<WeaponSystem> WeaponSystems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Ship entity
            modelBuilder.Entity<Ship>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name);

                // Configure one-to-one relationships
                entity.HasOne(e => e.Position)
                      .WithOne(p => p.Ship)
                      .HasForeignKey<Position>(p => p.ShipId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Hull)
                      .WithOne(h => h.Ship)
                      .HasForeignKey<Hull>(h => h.ShipId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Shield)
                      .WithOne(s => s.Ship)
                      .HasForeignKey<Shield>(s => s.ShipId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Engine)
                      .WithOne(e => e.Ship)
                      .HasForeignKey<Engine>(e => e.ShipId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.FuelSystem)
                      .WithOne(f => f.Ship)
                      .HasForeignKey<FuelSystem>(f => f.ShipId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.CargoSystem)
                      .WithOne(c => c.Ship)
                      .HasForeignKey<CargoSystem>(c => c.ShipId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.WeaponSystem)
                      .WithOne(w => w.Ship)
                      .HasForeignKey<WeaponSystem>(w => w.ShipId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
} 