using Microsoft.EntityFrameworkCore;
using SpacePirates.API.Data;
using SpacePirates.API.Models;

namespace SpacePirates.API.Services
{
    public class ShipService : IShipService
    {
        private readonly ApplicationDbContext _context;

        public ShipService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Ship> CreateShipAsync(Ship ship)
        {
            _context.Ships.Add(ship);
            await _context.SaveChangesAsync();
            return ship;
        }

        public async Task<Ship?> GetShipByIdAsync(int id)
        {
            return await _context.Ships
                .Include(s => s.Position)
                .Include(s => s.Hull)
                .Include(s => s.Shield)
                .Include(s => s.Engine)
                .Include(s => s.FuelSystem)
                .Include(s => s.CargoSystem)
                    .ThenInclude(cs => cs.CargoItems)
                        .ThenInclude(ci => ci.Resource)
                .Include(s => s.WeaponSystem)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Ship>> GetAllShipsAsync()
        {
            return await _context.Ships
                .Include(s => s.Position)
                .Include(s => s.Hull)
                .Include(s => s.Shield)
                .Include(s => s.Engine)
                .Include(s => s.FuelSystem)
                .Include(s => s.CargoSystem)
                    .ThenInclude(cs => cs.CargoItems)
                        .ThenInclude(ci => ci.Resource)
                .Include(s => s.WeaponSystem)
                .ToListAsync();
        }

        public async Task<Ship> UpdateShipAsync(Ship ship)
        {
            ship.UpdatedAt = DateTime.UtcNow;
            _context.Ships.Update(ship);
            await _context.SaveChangesAsync();
            return ship;
        }

        public async Task<bool> DeleteShipAsync(int id)
        {
            var ship = await _context.Ships.FindAsync(id);
            if (ship == null) return false;

            _context.Ships.Remove(ship);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MoveShipAsync(int shipId, double newX, double newY)
        {
            var ship = await _context.Ships.FindAsync(shipId);
            if (ship == null || !ship.CanOperate()) return false;

            var distance = Math.Sqrt(
                Math.Pow(newX - ship.Position.X, 2) + 
                Math.Pow(newY - ship.Position.Y, 2));
            
            var fuelNeeded = (int)(distance / ship.FuelSystem.Efficiency);

            if (ship.FuelSystem.CurrentFuel < fuelNeeded) return false;

            ship.Position.X = newX;
            ship.Position.Y = newY;
            ship.FuelSystem.CurrentFuel -= fuelNeeded;
            ship.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConsumeFuelAsync(int shipId, int amount)
        {
            var ship = await _context.Ships.FindAsync(shipId);
            if (ship == null || ship.FuelSystem.CurrentFuel < amount) return false;

            ship.FuelSystem.CurrentFuel -= amount;
            ship.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RefuelShipAsync(int shipId, int amount)
        {
            var ship = await _context.Ships.FindAsync(shipId);
            if (ship == null) return false;

            var maxFuel = ship.FuelSystem.CalculateMaxCapacity();
            ship.FuelSystem.CurrentFuel = Math.Min(ship.FuelSystem.CurrentFuel + amount, maxFuel);
            ship.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCargoAsync(int shipId, int cargoChange)
        {
            var ship = await _context.Ships.FindAsync(shipId);
            if (ship == null) return false;

            var newCargoLoad = ship.CargoSystem.CurrentLoad + cargoChange;
            if (newCargoLoad < 0 || newCargoLoad > ship.CargoSystem.CalculateMaxCapacity()) return false;

            ship.CargoSystem.CurrentLoad = newCargoLoad;
            ship.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TakeDamageAsync(int shipId, int damageAmount)
        {
            var ship = await _context.Ships.FindAsync(shipId);
            if (ship == null || ship.IsDestroyed) return false;

            var (shieldDamage, hullDamage) = ship.TakeDamage(damageAmount);
            ship.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RepairShipAsync(int shipId, int repairAmount)
        {
            var ship = await _context.Ships.FindAsync(shipId);
            if (ship == null || ship.IsDestroyed) return false;

            ship.Hull.Repair(repairAmount);
            ship.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCreditsAsync(int shipId, int creditChange)
        {
            var ship = await _context.Ships.FindAsync(shipId);
            if (ship == null) return false;

            var newCredits = ship.Credits + creditChange;
            if (newCredits < 0) return false;

            ship.Credits = newCredits;
            ship.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 