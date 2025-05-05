using System.ComponentModel.DataAnnotations;

namespace SpacePirates.API.Models
{
    public class Resource
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string ResourceType { get; set; } = string.Empty; // FuelOre, HullAlloy, etc.
        public double WeightPerUnit { get; set; }
        public string Description { get; set; } = string.Empty;
    }
} 