using System.Collections.Generic;

namespace SpacePirates.API.Models.DTOs
{
    public class GalaxyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<SolarSystemDto> SolarSystems { get; set; } = new();
    }
} 