using SpacePirates.API.Models;

namespace SpacePirates.API.Services
{
    public interface IShipService
    {
        // CRUD Operations
        Task<Ship> CreateShipAsync(Ship ship);
        Task<Ship?> GetShipByIdAsync(int id);
        Task<IEnumerable<Ship>> GetAllShipsAsync();
        Task<Ship> UpdateShipAsync(Ship ship);
        Task<bool> DeleteShipAsync(int id);

        // Game-Specific Operations
        Task<bool> MoveShipAsync(int shipId, double newX, double newY);
        Task<bool> ConsumeFuelAsync(int shipId, int amount);
        Task<bool> RefuelShipAsync(int shipId, int amount);
        Task<bool> UpdateCargoAsync(int shipId, int cargoChange);
        Task<bool> TakeDamageAsync(int shipId, int damageAmount);
        Task<bool> RepairShipAsync(int shipId, int repairAmount);
        Task<bool> UpdateCreditsAsync(int shipId, int creditChange);
    }
} 