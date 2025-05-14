using Microsoft.AspNetCore.Mvc;
using SpacePirates.API.Models;
using SpacePirates.API.Services;
using SpacePirates.API.Models.DTOs;

namespace SpacePirates.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipsController : ControllerBase
    {
        private readonly IShipService _shipService;

        public ShipsController(IShipService shipService)
        {
            _shipService = shipService;
        }

        [HttpPost]
        public async Task<ActionResult<Ship>> CreateShip(Ship ship)
        {
            var createdShip = await _shipService.CreateShipAsync(ship);
            return CreatedAtAction(nameof(GetShip), new { id = createdShip.Id }, createdShip);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShipDto>> GetShip(int id)
        {
            var ship = await _shipService.GetShipByIdAsync(id);
            if (ship == null) return NotFound();
            return Ok(MapToShipDto(ship));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipDto>>> GetAllShips()
        {
            var ships = await _shipService.GetAllShipsAsync();
            return Ok(ships.Select(MapToShipDto));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Ship>> UpdateShip(int id, Ship ship)
        {
            if (id != ship.Id) return BadRequest();
            return await _shipService.UpdateShipAsync(ship);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteShip(int id)
        {
            var result = await _shipService.DeleteShipAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/move")]
        public async Task<ActionResult> MoveShip(int id, [FromBody] MoveShipRequest request)
        {
            var result = await _shipService.MoveShipAsync(id, request.X, request.Y);
            if (!result) return BadRequest("Unable to move ship - check fuel levels");
            return Ok();
        }

        [HttpPost("{id}/refuel")]
        public async Task<ActionResult> RefuelShip(int id, [FromBody] RefuelRequest request)
        {
            var result = await _shipService.RefuelShipAsync(id, request.Amount);
            if (!result) return BadRequest("Unable to refuel ship");
            return Ok();
        }

        [HttpPost("{id}/cargo")]
        public async Task<ActionResult> UpdateCargo(int id, [FromBody] CargoUpdateRequest request)
        {
            var result = await _shipService.UpdateCargoAsync(id, request.CargoChange);
            if (!result) return BadRequest("Unable to update cargo - check capacity");
            return Ok();
        }

        [HttpPost("{id}/damage")]
        public async Task<ActionResult> TakeDamage(int id, [FromBody] DamageRequest request)
        {
            var result = await _shipService.TakeDamageAsync(id, request.Amount);
            if (!result) return BadRequest("Unable to process damage");
            return Ok();
        }

        [HttpPost("{id}/repair")]
        public async Task<ActionResult> RepairShip(int id, [FromBody] RepairRequest request)
        {
            var result = await _shipService.RepairShipAsync(id, request.Amount);
            if (!result) return BadRequest("Unable to repair ship");
            return Ok();
        }

        [HttpPost("{id}/credits")]
        public async Task<ActionResult> UpdateCredits(int id, [FromBody] CreditsUpdateRequest request)
        {
            var result = await _shipService.UpdateCreditsAsync(id, request.Amount);
            if (!result) return BadRequest("Unable to update credits");
            return Ok();
        }

        [HttpPost("{id}/upgrade-part")]
        public async Task<IActionResult> UpgradeShipPart(int id, [FromBody] UpgradePartRequest req)
        {
            // Load ship with all components and cargo
            var ship = await _shipService.GetShipByIdAsync(id);
            if (ship == null || ship.CargoSystem == null)
                return NotFound();

            // Map part name to component and required resource type
            string part = req.PartName.ToLower();
            int currentLevel = 0;
            string requiredResourceType = "";
            dynamic? component = null;

            switch (part)
            {
                case "engine":
                    component = ship.Engine;
                    currentLevel = ship.Engine.CurrentLevel;
                    requiredResourceType = "EngineParts";
                    break;
                case "hull":
                    component = ship.Hull;
                    currentLevel = ship.Hull.CurrentLevel;
                    requiredResourceType = "HullAlloy";
                    break;
                case "shield":
                    component = ship.Shield;
                    currentLevel = ship.Shield.CurrentLevel;
                    requiredResourceType = "ShieldPlasma";
                    break;
                case "weapon":
                case "weaponsystem":
                    component = ship.WeaponSystem;
                    currentLevel = ship.WeaponSystem.CurrentLevel;
                    requiredResourceType = "WeaponCrystal";
                    break;
                case "cargo":
                case "cargosystem":
                    component = ship.CargoSystem;
                    currentLevel = ship.CargoSystem.CurrentLevel;
                    requiredResourceType = "HullAlloy"; // Or another resource if you want
                    break;
                default:
                    return BadRequest("Invalid part name.");
            }

            // Calculate required amount
            int requiredAmount = currentLevel * 100;

            // Find the cargo item for the required resource
            var cargoItem = ship.CargoSystem.CargoItems
                .FirstOrDefault(ci => ci.Resource.ResourceType == requiredResourceType);

            if (cargoItem == null || cargoItem.Amount < requiredAmount)
                return BadRequest($"Not enough {requiredResourceType} to upgrade {part}. Required: {requiredAmount}");

            // Deduct the material and upgrade the part
            cargoItem.Amount -= requiredAmount;
            component.CurrentLevel += 1;

            // Save changes
            await _shipService.UpdateShipAsync(ship);

            return Ok(new
            {
                Message = $"{part} upgraded to level {component.CurrentLevel}.",
                NewLevel = component.CurrentLevel
            });
        }

        private ShipDto MapToShipDto(Ship ship)
        {
            return new ShipDto
            {
                Id = ship.Id,
                Name = ship.Name,
                CaptainName = ship.CaptainName,
                Credits = ship.Credits,
                Position = ship.Position == null ? null : new PositionDto
                {
                    X = ship.Position.X,
                    Y = ship.Position.Y
                },
                FuelSystem = ship.FuelSystem == null ? null : new FuelSystemDto
                {
                    CurrentLevel = ship.FuelSystem.CurrentLevel,
                    CurrentFuel = ship.FuelSystem.CurrentFuel
                },
                Shield = ship.Shield == null ? null : new ShieldDto
                {
                    CurrentLevel = ship.Shield.CurrentLevel,
                    CurrentIntegrity = ship.Shield.CurrentIntegrity,
                    IsActive = ship.Shield.IsActive
                },
                Hull = ship.Hull == null ? null : new HullDto
                {
                    CurrentLevel = ship.Hull.CurrentLevel,
                    CurrentIntegrity = ship.Hull.CurrentIntegrity
                },
                Engine = ship.Engine == null ? null : new EngineDto
                {
                    CurrentLevel = ship.Engine.CurrentLevel
                },
                CargoSystem = ship.CargoSystem == null ? null : new CargoSystemDto
                {
                    CurrentLevel = ship.CargoSystem.CurrentLevel,
                    CurrentLoad = ship.CargoSystem.CurrentLoad
                },
                WeaponSystem = ship.WeaponSystem == null ? null : new WeaponSystemDto
                {
                    CurrentLevel = ship.WeaponSystem.CurrentLevel
                }
            };
        }
    }

    public record MoveShipRequest(double X, double Y);
    public record RefuelRequest(int Amount);
    public record CargoUpdateRequest(int CargoChange);
    public record DamageRequest(int Amount);
    public record RepairRequest(int Amount);
    public record CreditsUpdateRequest(int Amount);
    public class UpgradePartRequest
    {
        public string PartName { get; set; } = "";
    }
} 