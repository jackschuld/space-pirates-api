using System.ComponentModel.DataAnnotations;

namespace SpacePirates.API.Models
{
    public class Star : CelestialObject
    {
        public string Type { get; set; } = string.Empty;
        // Add more star properties as needed (temperature, mass, etc.)
    }
} 