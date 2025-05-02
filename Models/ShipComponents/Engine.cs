namespace SpacePirates.API.Models.ShipComponents
{
    public class Engine : Level
    {
        public double MaxSpeed => CurrentLevel * 20; // 20 speed units per level
        public double Maneuverability => CurrentLevel * 0.2; // 0.2 maneuverability units per level
        
        public override int CalculateMaxCapacity()
        {
            return CurrentLevel * 20; // This represents max thrust capacity
        }
    }
} 