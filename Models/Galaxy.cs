using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpacePirates.API.Models
{
    public class Galaxy
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public List<SolarSystem> SolarSystems { get; set; } = new();
    }
} 