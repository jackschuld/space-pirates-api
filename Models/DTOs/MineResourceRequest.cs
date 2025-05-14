namespace SpacePirates.API.Models.DTOs
{
    public class MineResourceRequest
    {
        public int PlanetId { get; set; }
        public int ResourceId { get; set; }
        public int Amount { get; set; }
        public int ShipId { get; set; }
    }
} 