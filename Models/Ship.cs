using System.ComponentModel.DataAnnotations;
using SpacePirates.API.Models.ShipComponents;

namespace SpacePirates.API.Models
{
    public class Ship
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string CaptainName { get; set; } = string.Empty;

        // Components - One-to-One relationships
        public Position Position { get; set; } = null!;
        public Hull Hull { get; set; } = null!;
        public Shield Shield { get; set; } = null!;
        public Engine Engine { get; set; } = null!;
        public FuelSystem FuelSystem { get; set; } = null!;
        public CargoSystem CargoSystem { get; set; } = null!;
        public WeaponSystem WeaponSystem { get; set; } = null!;

        // Economy
        public int Credits { get; set; } = 1000;

        // Status
        public bool IsInCombat { get; set; }
        public bool IsDestroyed => Hull.IsDestroyed;
        public DateTime LastDocked { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Combat Methods
        public (int shieldDamage, int hullDamage) TakeDamage(int incomingDamage)
        {
            if (IsDestroyed) return (0, 0);

            // Auto-recharge shields before taking damage
            Shield.Recharge();

            // First, damage goes to shields
            int remainingDamage = Shield.AbsorbDamage(incomingDamage);
            int shieldDamage = incomingDamage - remainingDamage;

            // Any remaining damage goes to hull
            int hullDamage = 0;
            if (remainingDamage > 0)
            {
                hullDamage = Hull.TakeDamage(remainingDamage);
            }

            UpdatedAt = DateTime.UtcNow;
            return (shieldDamage, hullDamage);
        }

        public bool CanOperate()
        {
            return !IsDestroyed && 
                   FuelSystem.CurrentFuel > 0 && 
                   Hull.CurrentIntegrity > Hull.CalculateMaxCapacity() * 0.1; // Requires at least 10% hull integrity
        }

        public double CalculateCombatEffectiveness()
        {
            if (!CanOperate()) return 0;

            double hullIntegrityFactor = (double)Hull.CurrentIntegrity / Hull.CalculateMaxCapacity();
            double shieldIntegrityFactor = (double)Shield.CurrentIntegrity / Shield.CalculateMaxCapacity();
            
            return (hullIntegrityFactor * 0.4 + // Hull contributes 40%
                    shieldIntegrityFactor * 0.2 + // Shields contribute 20%
                    WeaponSystem.Accuracy * 0.2 + // Accuracy contributes 20%
                    (WeaponSystem.Damage / 100.0) * 0.2) * 100; // Damage contributes 20%
        }
    }
} 