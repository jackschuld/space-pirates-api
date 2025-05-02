namespace SpacePirates.API.Models.ShipComponents
{
    public class CargoSystem : Level
    {
        public int CurrentLoad { get; set; }
        
        public override int CalculateMaxCapacity()
        {
            return CurrentLevel * 100; // 100 cargo units per level
        }
    }
} 