using SpacePirates.API.Models;
using System;
using System.Collections.Generic;

namespace SpacePirates.API.Services
{
    public class GalaxyGenerator : IGalaxyGenerator
    {
        private static readonly string[] StarTypes = new[] { "Red Dwarf", "Yellow Dwarf", "Blue Giant", "White Dwarf", "Neutron Star", "Binary Star", "Red Giant" };
        private static readonly string[] TerrestrialTypes = new[] { "Terrestrial", "Desert", "Ice", "Oceanic" };
        private static readonly string[] GasGiantTypes = new[] { "Gas Giant", "Ice Giant" };
        private static readonly Random random = new();

        public Galaxy GenerateGalaxy(int numSystems, int minPlanets, int maxPlanets)
        {
            minPlanets = Math.Max(minPlanets, 3); // Ensure minimum of 3
            // For maxPlanets, use a reasonable upper bound for a circular orbit pattern
            // Assume radius = 100 (arbitrary), min spacing = 10 units
            int maxPlanetsAllowed = (int)(2 * Math.PI * 100 / 10); // ~62
            maxPlanets = Math.Min(maxPlanets, maxPlanetsAllowed);
            var galaxy = new Galaxy
            {
                Name = $"Galaxy-{Guid.NewGuid().ToString()[..4]}",
                SolarSystems = new List<SolarSystem>()
            };
            for (int i = 0; i < numSystems; i++)
            {
                double x = random.NextDouble() * 1000;
                double y = random.NextDouble() * 1000;
                var system = GenerateSolarSystem(i, x, y, minPlanets, maxPlanets);
                system.Galaxy = galaxy;
                system.GalaxyId = galaxy.Id;
                galaxy.SolarSystems.Add(system);
            }
            return galaxy;
        }

        private SolarSystem GenerateSolarSystem(int index, double x, double y, int minPlanets, int maxPlanets)
        {
            var starType = StarTypes[random.Next(StarTypes.Length)];
            var star = new Star
            {
                Name = $"Star-{Guid.NewGuid().ToString()[..4]}",
                Type = starType,
                X = x,
                Y = y,
                IsDiscovered = false
            };
            var system = new SolarSystem
            {
                Name = $"System-{Guid.NewGuid().ToString()[..4]}",
                X = x,
                Y = y,
                SunType = starType,
                Planets = new List<Planet>(),
                Star = star
            };
            int numPlanets = random.Next(minPlanets, maxPlanets + 1);
            double minOrbit = 10.0;
            double orbitStep = 10.0 + random.NextDouble() * 5.0;
            for (int i = 0; i < numPlanets; i++)
            {
                double distance = minOrbit + i * orbitStep + random.NextDouble() * 2.0;
                double angle = random.NextDouble() * 2 * Math.PI;
                double px = system.X + distance * Math.Cos(angle);
                double py = system.Y + distance * Math.Sin(angle);
                string planetType = (i < 2) ? TerrestrialTypes[random.Next(TerrestrialTypes.Length)] : GasGiantTypes[random.Next(GasGiantTypes.Length)];
                var planet = new Planet
                {
                    Name = $"Planet-{Guid.NewGuid().ToString()[..4]}",
                    PlanetType = planetType,
                    X = px,
                    Y = py,
                    Resources = new List<PlanetResource>(),
                    IsDiscovered = false
                };
                planet.SolarSystem = system;
                planet.SolarSystemId = system.Id;
                system.Planets.Add(planet);
            }
            return system;
        }
    }
} 