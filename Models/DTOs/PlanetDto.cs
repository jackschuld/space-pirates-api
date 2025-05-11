using System.Collections.Generic;

namespace SpacePirates.API.Models.DTOs
{
    public class CelestialObjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class PlanetDto : CelestialObjectDto
    {
        public string PlanetType { get; set; } = string.Empty;
        public List<PlanetResourceDto> Resources { get; set; } = new();
    }
} 