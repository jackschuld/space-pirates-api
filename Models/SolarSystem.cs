using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpacePirates.API.Models
{
    public class SolarSystem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public int GalaxyId { get; set; }
        public Galaxy Galaxy { get; set; } = null!;
        public double X { get; set; }
        public double Y { get; set; }
        public string SunType { get; set; } = "G"; // e.g. G, K, M, etc.
        public List<Planet> Planets { get; set; } = new();
        public int StarId { get; set; }
        public Star Star { get; set; } = null!;
    }
} 