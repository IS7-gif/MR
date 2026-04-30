using System;
using System.Collections.Generic;

namespace Project.Scripts.Shared.Passives
{
    public readonly struct HeroPassiveDefinition
    {
        public string DisplayName { get; }
        public ActivationConditionGroupDefinition ActivationConditions { get; }
        public IReadOnlyList<PassiveEffectEntryDefinition> EffectEntries =>
            _effectEntries ?? Array.Empty<PassiveEffectEntryDefinition>();
        public bool CanActivateWhileActive { get; }
        public int MaxActivations { get; }
        public bool IsConfigured => ActivationConditions.IsConfigured && HasConfiguredEffects();


        private readonly PassiveEffectEntryDefinition[] _effectEntries;


        public HeroPassiveDefinition(string displayName, ActivationConditionGroupDefinition activationConditions,
            IReadOnlyList<PassiveEffectEntryDefinition> effectEntries, bool canActivateWhileActive, int maxActivations)
        {
            DisplayName = displayName ?? string.Empty;
            ActivationConditions = activationConditions;
            _effectEntries = CopyConfiguredEffects(effectEntries);
            CanActivateWhileActive = canActivateWhileActive;
            MaxActivations = maxActivations < 0 ? 0 : maxActivations;
        }

        private bool HasConfiguredEffects()
        {
            if (_effectEntries != null)
                for (var i = 0; i < _effectEntries.Length; i++)
                    if (_effectEntries[i].IsConfigured)
                        return true;

            return false;
        }

        private static PassiveEffectEntryDefinition[] CopyConfiguredEffects(IReadOnlyList<PassiveEffectEntryDefinition> effects)
        {
            if (null == effects || effects.Count == 0)
                return Array.Empty<PassiveEffectEntryDefinition>();

            var result = new List<PassiveEffectEntryDefinition>(effects.Count);
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