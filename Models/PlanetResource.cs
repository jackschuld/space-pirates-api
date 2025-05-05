using System.ComponentModel.DataAnnotations;

namespace SpacePirates.API.Models
{
    public class PlanetResource
    {
        [Key]
        public int Id { get; set; }
        public int PlanetId { get; set; }
        public Planet Planet { get; set; } = null!;
        public int ResourceId { get; set; }
        public Resource Resource { get; set; } = null!;
        public int AmountAvailable { get; set; }
    }
} 