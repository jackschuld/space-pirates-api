namespace SpacePirates.API.Models.DTOs
{
    public class GameStateDto
    {
        public ShipDto Ship { get; set; } = null!;
        public GalaxyDto Galaxy { get; set; } = null!;
    }
} 