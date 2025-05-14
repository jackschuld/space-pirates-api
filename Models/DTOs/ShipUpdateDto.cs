namespace SpacePirates.API.Models.DTOs
{
    public class ShipUpdateDto
    {
        public PositionDto? Position { get; set; }
        public FuelSystemDto? FuelSystem { get; set; }
        public ShieldDto? Shield { get; set; }
        public HullDto? Hull { get; set; }
        public EngineDto? Engine { get; set; }
        public CargoSystemDto? CargoSystem { get; set; }
        public WeaponSystemDto? WeaponSystem { get; set; }
    }
} 