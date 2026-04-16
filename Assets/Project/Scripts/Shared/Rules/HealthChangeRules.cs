using System;

namespace Project.Scripts.Shared.Rules
{
    public readonly struct HealthChangeResult
    {
        public int PreviousHP { get; }
        public int CurrentHP { get; }
        public bool WasChanged => CurrentHP != PreviousHP;
        public bool BecameDefeated => PreviousHP > 0 && CurrentHP <= 0;


        public HealthChangeResult(int previousHP, int currentHP)
        {
            PreviousHP = previousHP;
            CurrentHP = currentHP;
        }
    }

    public static class HealthChangeRules
    {
        public static HealthChangeResult Apply(int currentHP, int maxHP, int delta, bool allowChangeWhenDefeated = false)
        {
            if (maxHP <= 0)
                return new HealthChangeResult(0, 0);

            var previousHP = Math.Clamp(currentHP, 0, maxHP);

            if (delta == 0)
                return new HealthChangeResult(previousHP, previousHP);

            if (false == allowChangeWhenDefeated && previousHP <= 0)
                return new HealthChangeResult(previousHP, previousHP);

            var nextHP = Math.Clamp(previousHP + delta, 0, maxHP);
            
            return new HealthChangeResult(previousHP, nextHP);
        }
    }
}