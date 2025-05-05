using System;
using System.ComponentModel.DataAnnotations;

namespace SpacePirates.API.Models
{
    public class GameSession
    {
        [Key]
        public int Id { get; set; }
        public int ShipId { get; set; }
        public Ship Ship { get; set; } = null!;
        public int GalaxyId { get; set; }
        public Galaxy Galaxy { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastPlayed { get; set; } = DateTime.UtcNow;
    }
} 