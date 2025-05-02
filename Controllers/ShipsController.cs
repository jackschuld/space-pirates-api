using Microsoft.AspNetCore.Mvc;
using SpacePirates.API.Models;
using SpacePirates.API.Services;

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
        public async Task<ActionResult<Ship>> GetShip(int id)
        {
            var ship = await _shipService.GetShipByIdAsync(id);
            if (ship == null) return NotFound();
            return ship;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ship>>> GetAllShips()
        {
            return Ok(await _shipService.GetAllShipsAsync());
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
    }

    public record MoveShipRequest(double X, double Y);
    public record RefuelRequest(int Amount);
    public record CargoUpdateRequest(int CargoChange);
    public record DamageRequest(int Amount);
    public record RepairRequest(int Amount);
    public record CreditsUpdateRequest(int Amount);
} 