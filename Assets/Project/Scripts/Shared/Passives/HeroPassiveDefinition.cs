namespace Project.Scripts.Shared.Passives
{
    public readonly struct HeroPassiveDefinition
    {
        public PassiveAbilityKind AbilityKind { get; }
        public PassiveConditionKind ConditionKind { get; }
        public int ConditionRequirement { get; }
        public int Power { get; }
        public bool AllowMultipleActivations { get; }
        public int ActivationLimit { get; }
        public float ConditionWindowSeconds { get; }

        public bool IsConfigured => AbilityKind != PassiveAbilityKind.None
                                    && ConditionKind != PassiveConditionKind.None;


        public HeroPassiveDefinition(
            PassiveAbilityKind abilityKind,
            PassiveConditionKind conditionKind,
            int conditionRequirement,
            int power,
            bool allowMultipleActivations,
            int activationLimit,
            float conditionWindowSeconds = 0f)
        {
            AbilityKind = abilityKind;
            ConditionKind = conditionKind;
            ConditionRequirement = conditionRequirement < 1 ? 1 : conditionRequirement;
            Power = power < 0 ? 0 : power;
            AllowMultipleActivations = allowMultipleActivations;
            ActivationLimit = activationLimit < 0 ? 0 : activationLimit;
            ConditionWindowSeconds = conditionWindowSeconds < 0f ? 0f : conditionWindowSeconds;
        }
    }
}