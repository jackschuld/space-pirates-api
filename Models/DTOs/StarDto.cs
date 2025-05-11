namespace SpacePirates.API.Models.DTOs
{
    public class StarDto : CelestialObjectDto
    {
        public string Type { get; set; } = string.Empty;
        public bool IsDiscovered { get; set; } // Added for API response
        // Add more star properties as needed
    }
} 