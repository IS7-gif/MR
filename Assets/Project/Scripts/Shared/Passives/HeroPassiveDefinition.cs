using System;
using System.Collections.Generic;

namespace Project.Scripts.Shared.Passives
{
    public readonly struct HeroPassiveDefinition
    {
        public string DisplayName { get; }
        public PassiveConditionGroupDefinition ConditionGroup { get; }
        public IReadOnlyList<PassiveModifierEffectDefinition> ModifierEffects =>
            _modifierEffects ?? Array.Empty<PassiveModifierEffectDefinition>();
        public IReadOnlyList<PassiveActionEffectDefinition> ActionEffects =>
            _actionEffects ?? Array.Empty<PassiveActionEffectDefinition>();
        public bool AllowMultipleActivations { get; }
        public int ActivationLimit { get; }

        public bool IsConfigured => ConditionGroup.IsConfigured && HasConfiguredEffects();


        private readonly PassiveModifierEffectDefinition[] _modifierEffects;
        private readonly PassiveActionEffectDefinition[] _actionEffects;


        public HeroPassiveDefinition(
            string displayName,
            PassiveConditionGroupDefinition conditionGroup,
            IReadOnlyList<PassiveModifierEffectDefinition> modifierEffects,
            IReadOnlyList<PassiveActionEffectDefinition> actionEffects,
            bool allowMultipleActivations,
            int activationLimit)
        {
            DisplayName = displayName ?? string.Empty;
            ConditionGroup = conditionGroup;
            _modifierEffects = CopyConfiguredModifiers(modifierEffects);
            _actionEffects = CopyConfiguredActions(actionEffects);
            AllowMultipleActivations = allowMultipleActivations;
            ActivationLimit = activationLimit < 0 ? 0 : activationLimit;
        }

        private bool HasConfiguredEffects()
        {
            if (_modifierEffects != null)
                for (var i = 0; i < _modifierEffects.Length; i++)
                    if (_modifierEffects[i].IsConfigured)
                        return true;

            if (_actionEffects != null)
                for (var i = 0; i < _actionEffects.Length; i++)
                    if (_actionEffects[i].IsConfigured)
                        return true;

            return false;
        }

        private static PassiveModifierEffectDefinition[] CopyConfiguredModifiers(
            IReadOnlyList<PassiveModifierEffectDefinition> effects)
        {
            if (effects == null || effects.Count == 0)
                return Array.Empty<PassiveModifierEffectDefinition>();

            var result = new List<PassiveModifierEffectDefinition>(effects.Count);
            for (var i = 0; i < effects.Count; i++)
            {
                var effect = effects[i];
                if (effect.IsConfigured)
                    result.Add(effect);
            }

            return result.ToArray();
        }

        private static PassiveActionEffectDefinition[] CopyConfiguredActions(
            IReadOnlyList<PassiveActionEffectDefinition> effects)
        {
            if (effects == null || effects.Count == 0)
                return Array.Empty<PassiveActionEffectDefinition>();

            var result = new List<PassiveActionEffectDefinition>(effects.Count);
            for (var i = 0; i < effects.Count; i++)
            {
                var effect = effects[i];
                if (effect.IsConfigured)
                    result.Add(effect);
            }

            return result.ToArray();
        }
    }
}