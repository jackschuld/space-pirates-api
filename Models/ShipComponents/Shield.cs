namespace SpacePirates.API.Models.ShipComponents
{
    public class Shield : Level
    {
        public int CurrentIntegrity { get; set; }
        public bool IsDown => CurrentIntegrity <= 0;
        public bool IsActive { get; set; }
        public double RechargeRate => CurrentLevel * 0.5; 
        public DateTime LastRechargeTime { get; private set; } = DateTime.UtcNow;
        public bool Charging { get; set; } = false;
        
        public override int CalculateMaxCapacity()
        {
            return CurrentLevel * 50; // Base shield capacity of 50 per level
        }

        public Shield()
        {
            CurrentIntegrity = CalculateMaxCapacity();
            IsActive = false;
        }

        public int AbsorbDamage(int incomingDamage)
        {
            int previousIntegrity = CurrentIntegrity;
            CurrentIntegrity = Math.Max(0, CurrentIntegrity - incomingDamage);
            return Math.Max(0, incomingDamage - previousIntegrity); // Returns damage that penetrated shield
        }

        public void Recharge()
        {
            var now = DateTime.UtcNow;
            var timeSinceLastRecharge = (now - LastRechargeTime).TotalSeconds;
            var rechargeAmount = (int)(RechargeRate * timeSinceLastRecharge);
            
            if (rechargeAmount > 0)
            {
                CurrentIntegrity = Math.Min(CalculateMaxCapacity(), CurrentIntegrity + rechargeAmount);
                LastRechargeTime = now;
            }
        }

        public void ForceRecharge(int amount)
        {
            CurrentIntegrity = Math.Min(CalculateMaxCapacity(), CurrentIntegrity + amount);
            LastRechargeTime = DateTime.UtcNow;
        }
    }
} 