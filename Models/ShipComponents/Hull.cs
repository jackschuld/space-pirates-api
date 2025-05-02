namespace SpacePirates.API.Models.ShipComponents
{
    public class Hull : Level
    {
        public int CurrentIntegrity { get; set; }
        public bool IsDestroyed => CurrentIntegrity <= 0;
        
        public override int CalculateMaxCapacity()
        {
            return CurrentLevel * 100; // Base hull integrity of 100 per level
        }

        public Hull()
        {
            CurrentIntegrity = CalculateMaxCapacity();
        }

        public int TakeDamage(int damageAmount)
        {
            int previousIntegrity = CurrentIntegrity;
            CurrentIntegrity = Math.Max(0, CurrentIntegrity - damageAmount);
            return previousIntegrity - CurrentIntegrity; // Returns actual damage taken
        }

        public int Repair(int repairAmount)
        {
            if (IsDestroyed) return 0; // Can't repair a destroyed hull

            int maxIntegrity = CalculateMaxCapacity();
            int previousIntegrity = CurrentIntegrity;
            CurrentIntegrity = Math.Min(maxIntegrity, CurrentIntegrity + repairAmount);
            return CurrentIntegrity - previousIntegrity; // Returns actual amount repaired
        }
    }
} 