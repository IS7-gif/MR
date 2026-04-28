namespace Project.Scripts.Shared.Passives
{
    public static class PassiveAbilityRules
    {
        public static bool HasSlotKindLinkedModifier(HeroPassiveDefinition definition)
        {
            var effects = definition.ModifierEffects;
            for (var i = 0; i < effects.Count; i++)
                if (effects[i].Target == PassiveModifierTarget.MatchEnergyBySlotKind)
                    return true;

            return false;
        }
    }
}