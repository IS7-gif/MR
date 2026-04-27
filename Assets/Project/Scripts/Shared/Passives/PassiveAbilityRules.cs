namespace Project.Scripts.Shared.Passives
{
    public static class PassiveAbilityRules
    {
        public static bool IsSlotKindLinked(PassiveAbilityKind kind)
        {
            return kind == PassiveAbilityKind.MatchEnergyBySlotKindPercent;
        }
    }
}
