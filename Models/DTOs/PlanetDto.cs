using System.Collections.Generic;

namespace SpacePirates.API.Models.DTOs
{
    public class PlanetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PlanetType { get; set; } = string.Empty;
        public List<PlanetResourceDto> Resources { get; set; } = new();
    }
} 