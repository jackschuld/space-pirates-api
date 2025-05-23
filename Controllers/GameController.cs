using Microsoft.AspNetCore.Mvc;
using SpacePirates.API.Data;
using SpacePirates.API.Models;
using SpacePirates.API.Models.ShipComponents;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpacePirates.API.Models.DTOs;
using SpacePirates.API.Services;
using Microsoft.Extensions.Logging;

namespace SpacePirates.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IGalaxyGenerator _galaxyGenerator;
        private readonly ILogger<GameController> _logger;
        public GameController(ApplicationDbContext db, IGalaxyGenerator galaxyGenerator, ILogger<GameController> logger)
        {
            _db = db;
            _galaxyGenerator = galaxyGenerator;
            _logger = logger;
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
            _db.Galaxies.Add(galaxy);
            await _db.SaveChangesAsync(); // Save first to get a valid Galaxy.Id

            var rand = new Random();
            int numSystems = rand.Next(5, 9);
            for (int i = 0; i < numSystems; i++)
            {
                // Create a star for the system
                string[] starTypes = { "G", "K", "M", "B", "O", "A", "F" };
                var starType = starTypes[rand.Next(starTypes.Length)];
                var star = new Star
                {
                    Name = $"Star-{Guid.NewGuid().ToString()[..4]}",
                    Type = starType,
                    X = rand.NextDouble() * 100,
                    Y = rand.NextDouble() * 100
                };
                _db.Stars.Add(star);
                await _db.SaveChangesAsync(); // Ensure the star gets an Id

                var system = new SolarSystem
                {
                    Name = $"System-{Guid.NewGuid().ToString()[..4]}",
                    X = star.X,
                    Y = star.Y,
                    SunType = star.Type,
                    StarId = star.Id,
                    Star = star,
                    GalaxyId = galaxy.Id, // Set the now-valid GalaxyId
                    Planets = new List<Planet>()
                };
                int numPlanets = rand.Next(2, 6);
                double minOrbit = 10.0;
                double orbitGap = 10.0; // Fixed gap between orbits
                for (int j = 0; j < numPlanets; j++)
                {
                    var planet = new Planet
                    {
                        Name = $"Planet-{Guid.NewGuid().ToString()[..4]}",
                        PlanetType = rand.Next(0, 2) == 0 ? "Terrestrial" : "Gas Giant",
                        X = 0, // Let the frontend determine the coordinates
                        Y = 0, // Let the frontend determine the coordinates
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
                _db.SolarSystems.Add(system);
            }
            await _db.SaveChangesAsync(); // Save all SolarSystems

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
                .FirstOrDefault();
            if (fullGalaxy != null)
            {
                var solarSystems = _db.SolarSystems
                    .Where(ss => ss.GalaxyId == fullGalaxy.Id)
                    .Include(ss => ss.Star)
                    .Include(ss => ss.Planets)
                        .ThenInclude(p => p.Resources)
                            .ThenInclude(r => r.Resource)
                    .ToList();
                fullGalaxy.SolarSystems = solarSystems;

                var galaxyDto = new GalaxyDto
                {
                    Id = fullGalaxy.Id,
                    Name = fullGalaxy.Name,
                    SolarSystems = solarSystems.Select(MapToSolarSystemDto).ToList()
                };

                return Ok(new GameStateDto
                {
                    Ship = fullShip == null ? null : new ShipDto
                    {
                        Id = fullShip.Id,
                        Name = fullShip.Name,
                        CaptainName = fullShip.CaptainName,
                        Credits = fullShip.Credits,
                        Position = fullShip.Position == null ? null : new PositionDto
                        {
                            X = fullShip.Position.X,
                            Y = fullShip.Position.Y
                        },
                        FuelSystem = fullShip.FuelSystem == null ? null : new FuelSystemDto
                        {
                            CurrentLevel = fullShip.FuelSystem.CurrentLevel,
                            CurrentFuel = fullShip.FuelSystem.CurrentFuel
                        },
                        Shield = fullShip.Shield == null ? null : new ShieldDto
                        {
                            CurrentLevel = fullShip.Shield.CurrentLevel,
                            CurrentIntegrity = fullShip.Shield.CurrentIntegrity,
                            IsActive = fullShip.Shield.IsActive
                        },
                        Hull = fullShip.Hull == null ? null : new HullDto
                        {
                            CurrentLevel = fullShip.Hull.CurrentLevel,
                            CurrentIntegrity = fullShip.Hull.CurrentIntegrity
                        },
                        Engine = fullShip.Engine == null ? null : new EngineDto
                        {
                            CurrentLevel = fullShip.Engine.CurrentLevel
                        },
                        CargoSystem = fullShip.CargoSystem == null ? null : new CargoSystemDto
                        {
                            CurrentLevel = fullShip.CargoSystem.CurrentLevel,
                            CurrentLoad = fullShip.CargoSystem.CurrentLoad,
                            MaxCapacity = fullShip.CargoSystem.CalculateMaxCapacity()
                        },
                        WeaponSystem = fullShip.WeaponSystem == null ? null : new WeaponSystemDto
                        {
                            CurrentLevel = fullShip.WeaponSystem.CurrentLevel
                        }
                    },
                    Galaxy = galaxyDto
                });
            }
            else
            {
                return NotFound();
            }
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
                .Include(s => s.Ship)
                    .ThenInclude(ship => ship.FuelSystem)
                .Include(s => s.Ship)
                    .ThenInclude(ship => ship.Shield)
                .Include(s => s.Ship)
                    .ThenInclude(ship => ship.Hull)
                .Include(s => s.Ship)
                    .ThenInclude(ship => ship.Engine)
                .Include(s => s.Ship)
                    .ThenInclude(ship => ship.CargoSystem)
                .Include(s => s.Ship)
                    .ThenInclude(ship => ship.WeaponSystem)
                .Include(s => s.Galaxy)
                .FirstOrDefault();
            List<SolarSystem> solarSystems = new List<SolarSystem>();
            if (session?.Galaxy != null)
            {
                solarSystems = _db.SolarSystems
                    .Where(ss => ss.GalaxyId == session.Galaxy.Id)
                    .Include(ss => ss.Star)
                    .Include(ss => ss.Planets)
                        .ThenInclude(p => p.Resources)
                            .ThenInclude(r => r.Resource)
                    .ToList();
                session.Galaxy.SolarSystems = solarSystems;
            }

            if (session == null) return NotFound();

            var galaxyDto = new GalaxyDto
            {
                Id = session.Galaxy.Id,
                Name = session.Galaxy.Name,
                SolarSystems = solarSystems.Select(MapToSolarSystemDto).ToList()
            };

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
                },
                FuelSystem = session.Ship.FuelSystem == null ? null : new FuelSystemDto
                {
                    CurrentLevel = session.Ship.FuelSystem.CurrentLevel,
                    CurrentFuel = session.Ship.FuelSystem.CurrentFuel
                },
                Shield = session.Ship.Shield == null ? null : new ShieldDto
                {
                    CurrentLevel = session.Ship.Shield.CurrentLevel,
                    CurrentIntegrity = session.Ship.Shield.CurrentIntegrity,
                    IsActive = session.Ship.Shield.IsActive
                },
                Hull = session.Ship.Hull == null ? null : new HullDto
                {
                    CurrentLevel = session.Ship.Hull.CurrentLevel,
                    CurrentIntegrity = session.Ship.Hull.CurrentIntegrity
                },
                Engine = session.Ship.Engine == null ? null : new EngineDto
                {
                    CurrentLevel = session.Ship.Engine.CurrentLevel
                },
                CargoSystem = session.Ship.CargoSystem == null ? null : new CargoSystemDto
                {
                    CurrentLevel = session.Ship.CargoSystem.CurrentLevel,
                    CurrentLoad = session.Ship.CargoSystem.CurrentLoad,
                    MaxCapacity = session.Ship.CargoSystem.CalculateMaxCapacity()
                },
                WeaponSystem = session.Ship.WeaponSystem == null ? null : new WeaponSystemDto
                {
                    CurrentLevel = session.Ship.WeaponSystem.CurrentLevel
                }
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

        // Example endpoint for generating a galaxy
        [HttpPost("/api/game/generate-galaxy")]
        public ActionResult<GalaxyDto> GenerateGalaxy([FromQuery] int numSystems = 10, [FromQuery] int minPlanets = 3, [FromQuery] int maxPlanets = 8)
        {
            var galaxy = _galaxyGenerator.GenerateGalaxy(numSystems, minPlanets, maxPlanets);
            // Map to DTOs (implement mapping as needed)
            var galaxyDto = MapToGalaxyDto(galaxy);
            return Ok(galaxyDto);
        }

        private GalaxyDto MapToGalaxyDto(Galaxy galaxy)
        {
            return new GalaxyDto
            {
                Id = galaxy.Id,
                Name = galaxy.Name,
                SolarSystems = galaxy.SolarSystems.Select(MapToSolarSystemDto).ToList()
            };
        }

        private SolarSystemDto MapToSolarSystemDto(SolarSystem system)
        {
            return new SolarSystemDto
            {
                Id = system.Id,
                Name = system.Name,
                X = system.X,
                Y = system.Y,
                SunType = system.SunType,
                Planets = system.Planets.Select(MapToPlanetDto).ToList(),
                Star = system.Star != null ? MapToStarDto(system.Star) : null
            };
        }

        private PlanetDto MapToPlanetDto(Planet planet)
        {
            return new PlanetDto
            {
                Id = planet.Id,
                Name = planet.Name,
                X = planet.X,
                Y = planet.Y,
                PlanetType = planet.PlanetType,
                Resources = planet.Resources.Select(r => new PlanetResourceDto
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
                }).ToList(),
                IsDiscovered = planet.IsDiscovered
            };
        }

        private StarDto MapToStarDto(Star star)
        {
            return new StarDto
            {
                Id = star.Id,
                Name = star.Name,
                X = star.X,
                Y = star.Y,
                Type = star.Type,
                IsDiscovered = star.IsDiscovered
            };
        }

        // POST: api/game/discover-star/{starId}
        [HttpPost("discover-star/{starId}")]
        public async Task<IActionResult> DiscoverStar(int starId)
        {
            _logger.LogInformation($"[DiscoverStar] Called with starId={starId}");
            var star = await _db.Stars.FindAsync(starId);
            if (star == null)
            {
                _logger.LogWarning($"[DiscoverStar] Star not found for starId={starId}");
                return NotFound();
            }
            star.IsDiscovered = true;
            await _db.SaveChangesAsync();
            _logger.LogInformation($"[DiscoverStar] Star {starId} marked as discovered.");
            return Ok();
        }

        // POST: api/game/inspect-planet/{planetId}
        [HttpPost("inspect-planet/{planetId}")]
        public async Task<IActionResult> InspectPlanet(int planetId)
        {
            _logger.LogInformation($"[InspectPlanet] Called with planetId={planetId}");
            var planet = await _db.Planets.FindAsync(planetId);
            if (planet == null)
            {
                _logger.LogWarning($"[InspectPlanet] Planet not found for planetId={planetId}");
                return NotFound();
            }
            planet.IsDiscovered = true;
            await _db.SaveChangesAsync();
            _logger.LogInformation($"[InspectPlanet] Planet {planetId} marked as discovered.");
            return Ok();
        }

        // GET: api/game/solar-system/{systemId}
        [HttpGet("solar-system/{systemId}")]
        public async Task<ActionResult<SolarSystemDto>> GetSolarSystem(int systemId)
        {
            var system = await _db.SolarSystems
                .Where(ss => ss.Id == systemId)
                .Include(ss => ss.Star)
                .Include(ss => ss.Planets)
                    .ThenInclude(p => p.Resources)
                        .ThenInclude(r => r.Resource)
                .FirstOrDefaultAsync();
            if (system == null)
                return NotFound();
            return Ok(MapToSolarSystemDto(system));
        }

        // POST: api/game/update-ship-fuel/{shipId}
        [HttpPost("update-ship-fuel/{shipId}")]
        public async Task<IActionResult> UpdateShipFuel(int shipId, [FromBody] double percentFuel)
        {
            var ship = await _db.Ships
                .Include(s => s.FuelSystem)
                .FirstOrDefaultAsync(s => s.Id == shipId);
            if (ship == null || ship.FuelSystem == null) return NotFound();
            ship.FuelSystem.CurrentFuel = percentFuel;
            await _db.SaveChangesAsync();
            return Ok();
        }

        // POST: api/game/update-ship-state/{shipId}
        [HttpPost("update-ship-state/{shipId}")]
        public async Task<IActionResult> UpdateShipState(int shipId, [FromBody] ShipUpdateDto dto)
        {
            var ship = await _db.Ships
                .Include(s => s.Position)
                .Include(s => s.FuelSystem)
                .Include(s => s.Shield)
                .Include(s => s.Hull)
                .Include(s => s.Engine)
                .Include(s => s.CargoSystem)
                .Include(s => s.WeaponSystem)
                .FirstOrDefaultAsync(s => s.Id == shipId);
            if (ship == null) return NotFound();

            // Update all mutable fields
            if (ship.Position != null && dto.Position != null)
            {
                ship.Position.X = dto.Position.X;
                ship.Position.Y = dto.Position.Y;
            }
            if (ship.FuelSystem != null && dto.FuelSystem != null)
                ship.FuelSystem.CurrentFuel = dto.FuelSystem.CurrentFuel;
            if (ship.Shield != null && dto.Shield != null)
                ship.Shield.CurrentIntegrity = dto.Shield.CurrentIntegrity;
            if (ship.Hull != null && dto.Hull != null)
                ship.Hull.CurrentIntegrity = dto.Hull.CurrentIntegrity;
            if (ship.Engine != null && dto.Engine != null)
                ship.Engine.CurrentLevel = dto.Engine.CurrentLevel;
            if (ship.CargoSystem != null && dto.CargoSystem != null)
                ship.CargoSystem.CurrentLoad = dto.CargoSystem.CurrentLoad;
            if (ship.WeaponSystem != null && dto.WeaponSystem != null)
                ship.WeaponSystem.CurrentLevel = dto.WeaponSystem.CurrentLevel;

            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("mine-planet-resource")]
        public async Task<IActionResult> MinePlanetResource([FromBody] MineResourceRequest req)
        {
            var planet = await _db.Planets
                .Include(p => p.Resources)
                    .ThenInclude(r => r.Resource)
                .FirstOrDefaultAsync(p => p.Id == req.PlanetId);
            var ship = await _db.Ships
                .Include(s => s.CargoSystem)
                    .ThenInclude(cs => cs.CargoItems)
                        .ThenInclude(ci => ci.Resource)
                .FirstOrDefaultAsync(s => s.Id == req.ShipId);

            if (planet == null || ship == null || ship.CargoSystem == null)
                return NotFound();

            var planetResource = planet.Resources.FirstOrDefault(r => r.ResourceId == req.ResourceId);
            if (planetResource == null || planetResource.AmountAvailable < req.Amount)
                return BadRequest("Not enough resource on planet.");

            // Subtract from planet
            planetResource.AmountAvailable -= req.Amount;

            // Add to ship cargo (by resource type)
            var cargoSystem = ship.CargoSystem;
            var cargoItem = cargoSystem.CargoItems.FirstOrDefault(ci => ci.ResourceId == req.ResourceId);
            if (cargoItem == null)
            {
                cargoItem = new SpacePirates.API.Models.ShipComponents.CargoItem
                {
                    ResourceId = req.ResourceId,
                    Resource = planetResource.Resource,
                    Amount = 0,
                    CargoSystem = cargoSystem
                };
                cargoSystem.CargoItems.Add(cargoItem);
            }
            cargoItem.Amount += req.Amount;
            // Update CurrentLoad as sum of all cargo item amounts
            cargoSystem.CurrentLoad = cargoSystem.CargoItems.Sum(ci => ci.Amount);

            await _db.SaveChangesAsync();

            // Prepare response DTOs
            var updatedPlanetResourceDto = new PlanetResourceDto
            {
                Id = planetResource.Id,
                Resource = new ResourceDto
                {
                    Id = planetResource.Resource.Id,
                    Name = planetResource.Resource.Name,
                    ResourceType = planetResource.Resource.ResourceType,
                    WeightPerUnit = planetResource.Resource.WeightPerUnit,
                    Description = planetResource.Resource.Description
                },
                AmountAvailable = planetResource.AmountAvailable
            };
            var cargoItemsDto = cargoSystem.CargoItems.Select(ci => new {
                Resource = new ResourceDto
                {
                    Id = ci.Resource.Id,
                    Name = ci.Resource.Name,
                    ResourceType = ci.Resource.ResourceType,
                    WeightPerUnit = ci.Resource.WeightPerUnit,
                    Description = ci.Resource.Description
                },
                Amount = ci.Amount
            }).ToList();

            return Ok(new {
                planetResource = updatedPlanetResourceDto,
                cargo = cargoItemsDto,
                currentLoad = cargoSystem.CurrentLoad
            });
        }
    }

    public class StartGameRequest
    {
        public string CaptainName { get; set; } = string.Empty;
        public string ShipName { get; set; } = string.Empty;
    }
} 