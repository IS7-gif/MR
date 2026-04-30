namespace Project.Scripts.Shared.Passives
{
    public enum PassiveModifierTarget
    {
        None,
        AbilityPower,
        ActivationEnergyCost,
        MatchEnergyBySlotKind
    }

    public enum PassiveModifierOperation
    {
        None,
        AddFlat,
        AddPercent
    }

    public readonly struct PassiveModifierEffectDefinition
    {
        public PassiveModifierTarget Target { get; }
        public PassiveModifierOperation Operation { get; }
        public float Value { get; }

        public bool IsConfigured => Target != PassiveModifierTarget.None
                                    && Operation != PassiveModifierOperation.None;


        public PassiveModifierEffectDefinition(
            PassiveModifierTarget target,
            PassiveModifierOperation operation,
            float value)
        {
            Target = target;
            Operation = operation;
            Value = value;
        }
    }

    public static class PassiveModifierRules
    {
        public static float Apply(float currentValue, PassiveModifierEffectDefinition effect, int activeStackCount)
        {
            if (activeStackCount <= 0 || false == effect.IsConfigured)
                return currentValue;

            var result = currentValue;
            for (var i = 0; i < activeStackCount; i++)
            {
                if (effect.Operation == PassiveModifierOperation.AddFlat)
                    result += effect.Value;
                else if (effect.Operation == PassiveModifierOperation.AddPercent)
                    result *= 1f + effect.Value / 100f;
            }

            return result;
        }
    }

    public enum PassiveActionEffectKind
    {
        None,
        GrantNextAttackBuff
    }

    public readonly struct PassiveActionEffectDefinition
    {
        public PassiveActionEffectKind Kind { get; }
        public float Value { get; }
        public int Count { get; }
        public PassiveUnitTargetDefinition Target { get; }

        public bool IsConfigured => Kind != PassiveActionEffectKind.None
                                    && Target.IsConfigured;


        public PassiveActionEffectDefinition(PassiveActionEffectKind kind, float value, int count,
            PassiveUnitTargetDefinition target)
        {
            Kind = kind;
            Value = value;
            Count = count < 0 ? 0 : count;
            Target = target;
        }
    }
}