using System.ComponentModel.DataAnnotations;

namespace SpacePirates.API.Models
{
    public abstract class Level
    {
        [Key]
        public int Id { get; set; }
        public int CurrentLevel { get; set; } = 1;
        public int MaxLevel { get; set; } = 10;
        public abstract int CalculateMaxCapacity();
        public DateTime LastUpgraded { get; set; } = DateTime.UtcNow;
        public int UpgradeCost => CurrentLevel * 1000; // Base upgrade cost calculation

        // Navigation properties
        public int ShipId { get; set; }
        public Ship Ship { get; set; } = null!;
    }
} 