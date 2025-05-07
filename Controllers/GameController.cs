using Microsoft.AspNetCore.Mvc;
using SpacePirates.API.Data;
using SpacePirates.API.Models;
using SpacePirates.API.Models.ShipComponents;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpacePirates.API.Models.DTOs;

namespace SpacePirates.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public GameController(ApplicationDbContext db)
        {
            _db = db;
        }

        // POST: api/game/start
        [HttpPost("start")]
        public async Task<IActionResult> StartNewGame([FromBody] StartGameRequest request)
        {
            // 1. Create resources if not already present
            var resourceTypes = new[]
            {
                new Resource { Name = "Fuel Ore", ResourceType = "FuelOre", WeightPerUnit = 1, Description = "Used to refine fuel." },
                new Resource { Name = "Hull Alloy", ResourceType = "HullAlloy", WeightPerUnit = 2, Description = "Used to repair or upgrade hull." },
                new Resource { Name = "Weapon Crystal", ResourceType = "WeaponCrystal", WeightPerUnit = 1.5, Description = "Used to upgrade weapons." },
                new Resource { Name = "Shield Plasma", ResourceType = "ShieldPlasma", WeightPerUnit = 1.2, Description = "Used to strengthen shields." },
                new Resource { Name = "Engine Parts", ResourceType = "EngineParts", WeightPerUnit = 3, Description = "Used to improve engines." }
            };
            foreach (var res in resourceTypes)
            {
                if (!_db.Resources.Any(r => r.ResourceType == res.ResourceType))
                    _db.Resources.Add(res);
            }
            await _db.SaveChangesAsync();
            var allResources = _db.Resources.ToList();

            // 2. Generate random galaxy
            var galaxy = new Galaxy { Name = $"Galaxy-{Guid.NewGuid().ToString()[..6]}" };
            var rand = new Random();
            int numSystems = rand.Next(5, 9);
            for (int i = 0; i < numSystems; i++)
            {
                var system = new SolarSystem
                {
                    Name = $"System-{Guid.NewGuid().ToString()[..4]}",
                    X = rand.NextDouble() * 100,
                    Y = rand.NextDouble() * 100,
                    SunType = rand.Next(0, 3) switch { 0 => "G", 1 => "K", _ => "M" },
                };
                int numPlanets = rand.Next(2, 6);
                for (int j = 0; j < numPlanets; j++)
                {
                    var planet = new Planet
                    {
                        Name = $"Planet-{Guid.NewGuid().ToString()[..4]}",
                        PlanetType = rand.Next(0, 2) == 0 ? "Terrestrial" : "Gas Giant",
                    };
                    // Add random resources to planet
                    int numRes = rand.Next(1, 4);
                    var picked = allResources.OrderBy(_ => rand.Next()).Take(numRes).ToList();
                    foreach (var res in picked)
                    {
                        planet.Resources.Add(new PlanetResource
                        {
                            ResourceId = res.Id,
                            Resource = res,
                            AmountAvailable = rand.Next(50, 200)
                        });
                    }
                    system.Planets.Add(planet);
                }
                galaxy.SolarSystems.Add(system);
            }
            _db.Galaxies.Add(galaxy);
            await _db.SaveChangesAsync();

            // 3. Create ship and components
            var ship = new Ship
            {
                Name = request.ShipName,
                CaptainName = request.CaptainName,
                Position = new Position { X = galaxy.SolarSystems[0].X, Y = galaxy.SolarSystems[0].Y },
                Hull = new Hull { CurrentLevel = 1, CurrentIntegrity = 100 },
                Shield = new Shield { CurrentLevel = 1, CurrentIntegrity = 100 },
                Engine = new Engine { CurrentLevel = 1 },
                FuelSystem = new FuelSystem { CurrentLevel = 1 },
                CargoSystem = new CargoSystem { CurrentLevel = 1, CurrentLoad = 0 },
                WeaponSystem = new WeaponSystem { CurrentLevel = 1 }
            };
            _db.Ships.Add(ship);
            await _db.SaveChangesAsync();

            // 4. Create game session
            var session = new GameSession
            {
                ShipId = ship.Id,
                Ship = ship,
                GalaxyId = galaxy.Id,
                Galaxy = galaxy,
                CreatedAt = DateTime.UtcNow,
                LastPlayed = DateTime.UtcNow
            };
            _db.GameSessions.Add(session);
            await _db.SaveChangesAsync();

            // Fetch the full ship and galaxy with all nested data
            var fullShip = _db.Ships
                .Where(s => s.Id == ship.Id)
                .Include(s => s.Position)
                .FirstOrDefault();

            var fullGalaxy = _db.Galaxies
                .Where(g => g.Id == galaxy.Id)
                .Include(g => g.SolarSystems)
                    .ThenInclude(sys => sys.Planets)
                        .ThenInclude(p => p.Resources)
                            .ThenInclude(r => r.Resource)
                .FirstOrDefault();

            var shipDto = new ShipDto
            {
                Id = fullShip.Id,
                Name = fullShip.Name,
                CaptainName = fullShip.CaptainName,
                Credits = fullShip.Credits,
                Position = fullShip.Position == null ? null : new PositionDto
                {
                    X = fullShip.Position.X,
                    Y = fullShip.Position.Y
                }
            };

            var galaxyDto = new GalaxyDto
            {
                Id = fullGalaxy.Id,
                Name = fullGalaxy.Name,
                SolarSystems = fullGalaxy.SolarSystems.Select(sys => new SolarSystemDto
                {
                    Id = sys.Id,
                    Name = sys.Name,
                    X = sys.X,
                    Y = sys.Y,
                    SunType = sys.SunType,
                    Planets = sys.Planets.Select(p => new PlanetDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        PlanetType = p.PlanetType,
                        Resources = p.Resources.Select(r => new PlanetResourceDto
                        {
                            Id = r.Id,
                            AmountAvailable = r.AmountAvailable,
                            Resource = new ResourceDto
                            {
                                Id = r.Resource.Id,
                                Name = r.Resource.Name,
                                ResourceType = r.Resource.ResourceType,
                                WeightPerUnit = r.Resource.WeightPerUnit,
                                Description = r.Resource.Description
                            }
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            return Ok(new GameStateDto
            {
                Ship = shipDto,
                Galaxy = galaxyDto
            });
        }

        // GET: api/game/list
        [HttpGet("list")]
        public IActionResult ListGames()
        {
            var sessions = _db.GameSessions
                .Select(s => new {
                    gameId = s.Id,
                    shipName = s.Ship.Name,
                    captainName = s.Ship.CaptainName,
                    galaxyName = s.Galaxy.Name,
                    lastPlayed = s.LastPlayed
                })
                .OrderByDescending(s => s.lastPlayed)
                .ToList();
            return Ok(sessions);
        }

        // GET: api/game/{id}
        [HttpGet("{id}")]
        public IActionResult LoadGame(int id)
        {
            var session = _db.GameSessions
                .Where(s => s.Id == id)
                .Include(s => s.Ship)
                    .ThenInclude(ship => ship.Position)
                .Include(s => s.Galaxy)
                    .ThenInclude(g => g.SolarSystems)
                        .ThenInclude(sys => sys.Planets)
                            .ThenInclude(p => p.Resources)
                                .ThenInclude(r => r.Resource)
                .FirstOrDefault();

            if (session == null) return NotFound();

            var shipDto = new ShipDto
            {
                Id = session.Ship.Id,
                Name = session.Ship.Name,
                CaptainName = session.Ship.CaptainName,
                Credits = session.Ship.Credits,
                Position = session.Ship.Position == null ? null : new PositionDto
                {
                    X = session.Ship.Position.X,
                    Y = session.Ship.Position.Y
                }
            };

            var galaxyDto = new GalaxyDto
            {
                Id = session.Galaxy.Id,
                Name = session.Galaxy.Name,
                SolarSystems = session.Galaxy.SolarSystems.Select(sys => new SolarSystemDto
                {
                    Id = sys.Id,
                    Name = sys.Name,
                    X = sys.X,
                    Y = sys.Y,
                    SunType = sys.SunType,
                    Planets = sys.Planets.Select(p => new PlanetDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        PlanetType = p.PlanetType,
                        Resources = p.Resources.Select(r => new PlanetResourceDto
                        {
                            Id = r.Id,
                            AmountAvailable = r.AmountAvailable,
                            Resource = new ResourceDto
                            {
                                Id = r.Resource.Id,
                                Name = r.Resource.Name,
                                ResourceType = r.Resource.ResourceType,
                                WeightPerUnit = r.Resource.WeightPerUnit,
                                Description = r.Resource.Description
                            }
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            return Ok(new GameStateDto
            {
                Ship = shipDto,
                Galaxy = galaxyDto
            });
        }

        // DELETE: api/game/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var session = _db.GameSessions.FirstOrDefault(s => s.Id == id);
            if (session == null) return NotFound();
            _db.GameSessions.Remove(session);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }

    public class StartGameRequest
    {
        public string CaptainName { get; set; } = string.Empty;
        public string ShipName { get; set; } = string.Empty;
    }
} 