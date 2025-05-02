namespace SpacePirates.API.Models.ShipComponents
{
    public class FuelSystem : Level
    {
        public int CurrentFuel { get; set; }
        public double Efficiency => 1 + (CurrentLevel * 0.1); // 10% better efficiency per level
        
        public override int CalculateMaxCapacity()
        {
            return CurrentLevel * 200; // 200 fuel units per level
        }

        public FuelSystem()
        {
            CurrentFuel = CalculateMaxCapacity();
        }
    }
} 