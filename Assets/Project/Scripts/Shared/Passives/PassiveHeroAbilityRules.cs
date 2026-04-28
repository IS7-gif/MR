using System;
using System.Collections.Generic;
using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Shared.Passives
{
    public static class PassiveHeroAbilityRules
    {
        public static int GetModifiedActivationEnergyCost(int baseCost, IReadOnlyList<HeroPassiveRuntimeState> passives,
            BattleSide side, int slotIndex)
        {
            if (baseCost <= 0)
                return 0;

            var cost = GetModifiedValue(
                baseCost,
                passives,
                side,
                slotIndex,
                PassiveModifierTarget.ActivationEnergyCost);

            return cost <= 0f ? 0 : (int)Math.Ceiling(cost);
        }

        public static int GetModifiedAbilityPower(int basePower, IReadOnlyList<HeroPassiveRuntimeState> passives,
            BattleSide side, int slotIndex)
        {
            var power = GetModifiedValue(
                basePower,
                passives,
                side,
                slotIndex,
                PassiveModifierTarget.AbilityPower);

            return power <= 0f ? 0 : (int)Math.Ceiling(power);
        }

        private static float GetModifiedValue(float baseValue, IReadOnlyList<HeroPassiveRuntimeState> passives,
            BattleSide side, int slotIndex, PassiveModifierTarget target)
        {
            var result = baseValue;
            if (passives == null)
                return result;

            for (var i = 0; i < passives.Count; i++)
            {
                var passive = passives[i];
                if (false == IsActiveOwnerPassive(passive, side, slotIndex))
                    continue;

                var effects = passive.Definition.ModifierEffects;
                for (var j = 0; j < effects.Count; j++)
                {
                    var effect = effects[j];
                    if (effect.Target == target)
                        result = PassiveModifierRules.Apply(result, effect, passive.ActivationCount);
                }
            }

            return result;
        }


        private static bool IsActiveOwnerPassive(HeroPassiveRuntimeState passive, BattleSide side, int slotIndex)
        {
            return passive.Side == side
                   && passive.SlotIndex == slotIndex
                   && passive is { IsActive: true, IsDisabled: false };
        }

    }
}