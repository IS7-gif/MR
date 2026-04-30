using System.Collections.Generic;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Tiles;

namespace Project.Scripts.Shared.Passives
{
    public static class PassiveEnergyRules
    {
        public static float SumMatchEnergyWithPassives(
            IReadOnlyDictionary<TileKind, float> energyByKind,
            IReadOnlyList<HeroPassiveRuntimeState> passives,
            BattleSide side)
        {
            if (energyByKind == null)
                return 0f;

            var total = 0f;
            foreach (var pair in energyByKind)
            {
                if (pair.Value <= 0f)
                    continue;

                total += GetModifiedMatchEnergy(pair.Value, passives, side, pair.Key);
            }

            return total;
        }

        public static float GetModifiedMatchEnergy(
            float baseEnergy,
            IReadOnlyList<HeroPassiveRuntimeState> passives,
            BattleSide side,
            TileKind tileKind)
        {
            var result = baseEnergy;
            if (null != passives)
            {
                for (var i = 0; i < passives.Count; i++)
                {
                    var passive = passives[i];
                    if (false == IsMatchEnergyPassive(passive, side, tileKind))
                        continue;

                    var effects = passive.Definition.ModifierEffects;
                    for (var j = 0; j < effects.Count; j++)
                    {
                        var effect = effects[j];
                        if (effect.Target == PassiveModifierTarget.MatchEnergyBySlotKind)
                            result = PassiveModifierRules.Apply(result, effect, passive.ActiveStackCount);
                    }
                }
            }

            return result < 0f ? 0f : result;
        }

        private static bool IsMatchEnergyPassive(
            HeroPassiveRuntimeState passive,
            BattleSide side,
            TileKind tileKind)
        {
            return passive.Side == side
                   && passive is { IsActive: true, IsDisabled: false }
                   && passive.SlotKind == tileKind;
        }
    }
}