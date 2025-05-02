using System.ComponentModel.DataAnnotations;

namespace SpacePirates.API.Models
{
    public class Position
    {
        [Key]
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        // Navigation properties
        public int ShipId { get; set; }
        public Ship Ship { get; set; } = null!;

        public double CalculateDistanceTo(Position other)
        {
            return Math.Sqrt(Math.Pow(other.X - X, 2) + Math.Pow(other.Y - Y, 2));
        }
    }
} 