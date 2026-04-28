using System;
using System.Collections.Generic;

namespace Project.Scripts.Shared.Passives
{
    public enum PassiveConditionGroupOperator
    {
        And,
        Or
    }

    public readonly struct PassiveConditionDefinition
    {
        public PassiveConditionKind Kind { get; }
        public int RequiredValue { get; }
        public float WindowSeconds { get; }

        public bool IsConfigured => Kind != PassiveConditionKind.None;


        public PassiveConditionDefinition(PassiveConditionKind kind, int requiredValue, float windowSeconds)
        {
            Kind = kind;
            RequiredValue = requiredValue < 1 ? 1 : requiredValue;
            WindowSeconds = windowSeconds < 0f ? 0f : windowSeconds;
        }
    }

    public readonly struct PassiveConditionGroupDefinition
    {
        public PassiveConditionGroupOperator Operator { get; }
        public IReadOnlyList<PassiveConditionDefinition> Conditions => _conditions ?? Array.Empty<PassiveConditionDefinition>();

        public bool IsConfigured => HasConfiguredConditions();


        private readonly PassiveConditionDefinition[] _conditions;


        public PassiveConditionGroupDefinition(
            PassiveConditionGroupOperator operatorKind,
            IReadOnlyList<PassiveConditionDefinition> conditions)
        {
            Operator = operatorKind;
            _conditions = CopyConfiguredConditions(conditions);
        }

        private bool HasConfiguredConditions()
        {
            if (_conditions == null)
                return false;

            for (var i = 0; i < _conditions.Length; i++)
                if (_conditions[i].IsConfigured)
                    return true;

            return false;
        }

        private static PassiveConditionDefinition[] CopyConfiguredConditions(IReadOnlyList<PassiveConditionDefinition> conditions)
        {
            if (conditions == null || conditions.Count == 0)
                return Array.Empty<PassiveConditionDefinition>();

            var result = new List<PassiveConditionDefinition>(conditions.Count);
            for (var i = 0; i < conditions.Count; i++)
            {
                var condition = conditions[i];
                if (condition.IsConfigured)
                    result.Add(condition);
            }

            return result.ToArray();
        }
    }

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
        AddPercent,
        Multiply
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
        public static float Apply(float currentValue, PassiveModifierEffectDefinition effect, int activationCount)
        {
            if (activationCount <= 0 || false == effect.IsConfigured)
                return currentValue;

            var result = currentValue;
            for (var i = 0; i < activationCount; i++)
            {
                if (effect.Operation == PassiveModifierOperation.AddFlat)
                    result += effect.Value;
                else if (effect.Operation == PassiveModifierOperation.AddPercent)
                    result *= 1f + effect.Value / 100f;
                else if (effect.Operation == PassiveModifierOperation.Multiply)
                    result *= effect.Value;
            }

            return result;
        }
    }

    public enum PassiveActionEffectKind
    {
        None,
        RepeatAbilityOnAdditionalTargets,
        RepeatAbilityOnSameTarget,
        RepeatLineSpecialOnAdjacentLines,
        ResurrectOnDeath,
        GrantTeamAbilityPowerUntilNextActivation
    }

    public readonly struct PassiveActionEffectDefinition
    {
        public PassiveActionEffectKind Kind { get; }
        public float Value { get; }
        public int Count { get; }

        public bool IsConfigured => Kind != PassiveActionEffectKind.None;


        public PassiveActionEffectDefinition(PassiveActionEffectKind kind, float value, int count)
        {
            Kind = kind;
            Value = value;
            Count = count < 0 ? 0 : count;
        }
    }
}
