using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpacePirates.API.Models
{
    public class Planet : CelestialObject
    {
        public int SolarSystemId { get; set; }
        public SolarSystem SolarSystem { get; set; } = null!;
        public string PlanetType { get; set; } = "Terrestrial"; // e.g. Terrestrial, Gas Giant, etc.
        public List<PlanetResource> Resources { get; set; } = new();
    }
} 