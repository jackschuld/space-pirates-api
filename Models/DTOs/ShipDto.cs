namespace SpacePirates.API.Models.DTOs
{
    public class ShipDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CaptainName { get; set; } = string.Empty;
        public int Credits { get; set; }
        public PositionDto? Position { get; set; }
        // Add other properties as needed, but avoid navigation properties that cause cycles
    }
} 