using SpacePirates.API.Models;

namespace SpacePirates.API.Services
{
    public interface IGalaxyGenerator
    {
        Galaxy GenerateGalaxy(int numSystems, int minPlanets, int maxPlanets);
    }
} 