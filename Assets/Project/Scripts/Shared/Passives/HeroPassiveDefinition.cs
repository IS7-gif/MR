using System;
using System.Collections.Generic;

namespace Project.Scripts.Shared.Passives
{
    public readonly struct HeroPassiveDefinition
    {
        public string DisplayName { get; }
        public PassiveTriggerKind TriggerKind { get; }
        public int RequiredTriggerCount { get; }
        public IReadOnlyList<PassiveModifierEffectDefinition> ModifierEffects =>
            _modifierEffects ?? Array.Empty<PassiveModifierEffectDefinition>();
        public IReadOnlyList<PassiveActionEffectDefinition> ActionEffects =>
            _actionEffects ?? Array.Empty<PassiveActionEffectDefinition>();
        public bool CanActivateWhileActive { get; }
        public int MaxActivations { get; }
        public int ActiveDurationRounds { get; }

        public bool IsConfigured => TriggerKind != PassiveTriggerKind.None && HasConfiguredEffects();


        private readonly PassiveModifierEffectDefinition[] _modifierEffects;
        private readonly PassiveActionEffectDefinition[] _actionEffects;


        public HeroPassiveDefinition(
            string displayName,
            PassiveTriggerKind triggerKind,
            int requiredTriggerCount,
            IReadOnlyList<PassiveModifierEffectDefinition> modifierEffects,
            IReadOnlyList<PassiveActionEffectDefinition> actionEffects,
            bool canActivateWhileActive,
            int maxActivations,
            int activeDurationRounds)
        {
            DisplayName = displayName ?? string.Empty;
            TriggerKind = triggerKind;
            RequiredTriggerCount = requiredTriggerCount < 1 ? 1 : requiredTriggerCount;
            _modifierEffects = CopyConfiguredModifiers(modifierEffects);
            _actionEffects = CopyConfiguredActions(actionEffects);
            CanActivateWhileActive = canActivateWhileActive;
            MaxActivations = maxActivations < 0 ? 0 : maxActivations;
            ActiveDurationRounds = activeDurationRounds < 0 ? 0 : activeDurationRounds;
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