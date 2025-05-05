namespace SpacePirates.API.Models.ShipComponents
{
    public class FuelSystem : Level
    {
        // Modular constants for tuning
        public const int BaseCapacityPerLevel = 200; // Each level adds this much capacity
        public const double BaseEfficiency = 1.0;    // Base efficiency multiplier
        public const double EfficiencyPerLevel = 0.1; // Each level adds this much efficiency

        public double CurrentFuel { get; set; }
        /// <summary>
        /// Efficiency multiplier: higher = less fuel used per move. Modular and easy to tune.
        /// </summary>
        public double Efficiency => BaseEfficiency + (CurrentLevel * EfficiencyPerLevel);
        
        /// <summary>
        /// Maximum fuel capacity. Modular and easy to tune.
        /// </summary>
        public override int CalculateMaxCapacity()
        {
            return CurrentLevel * BaseCapacityPerLevel;
        }

        public FuelSystem()
        {
            CurrentFuel = CalculateMaxCapacity(); // Always start at 100% fuel
        }
    }
} 