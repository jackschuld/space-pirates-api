using System.ComponentModel.DataAnnotations;

namespace SpacePirates.API.Models
{
    public abstract class CelestialObject
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public double X { get; set; }
        public double Y { get; set; }
        public bool IsDiscovered { get; set; } = false;
    }
} 