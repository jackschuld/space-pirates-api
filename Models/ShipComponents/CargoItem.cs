using System.ComponentModel.DataAnnotations;

namespace SpacePirates.API.Models.ShipComponents
{
    public class CargoItem
    {
        [Key]
        public int Id { get; set; }
        public int CargoSystemId { get; set; }
        public CargoSystem CargoSystem { get; set; } = null!;
        public int ResourceId { get; set; }
        public Resource Resource { get; set; } = null!;
        public int Amount { get; set; }
    }
} 