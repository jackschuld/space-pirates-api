namespace SpacePirates.API.Models.DTOs
{
    public class PlanetResourceDto
    {
        public int Id { get; set; }
        public ResourceDto Resource { get; set; } = null!;
        public int AmountAvailable { get; set; }
    }
} 