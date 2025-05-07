using System.Collections.Generic;

namespace SpacePirates.API.Models.DTOs
{
    public class SolarSystemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double X { get; set; }
        public double Y { get; set; }
        public string SunType { get; set; } = string.Empty;
        public List<PlanetDto> Planets { get; set; } = new();
    }
} 