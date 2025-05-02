namespace SpacePirates.API.Models.ShipComponents
{
    public class WeaponSystem : Level
    {
        public double Damage => CurrentLevel * 25; // 25 damage per level
        public double Accuracy => Math.Min(0.95, 0.6 + (CurrentLevel * 0.05)); // Starts at 60%, gains 5% per level, caps at 95%
        public double CriticalChance => Math.Min(0.5, CurrentLevel * 0.05); // 5% crit chance per level, caps at 50%
        
        public override int CalculateMaxCapacity()
        {
            return CurrentLevel * 25; // This represents max damage potential
        }
    }
} 