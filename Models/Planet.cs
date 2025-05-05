using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpacePirates.API.Models
{
    public class Planet
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public int SolarSystemId { get; set; }
        public SolarSystem SolarSystem { get; set; } = null!;
        public string PlanetType { get; set; } = "Terrestrial"; // e.g. Terrestrial, Gas Giant, etc.
        public List<PlanetResource> Resources { get; set; } = new();
    }
} 