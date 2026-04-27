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

                total += pair.Value * GetMatchEnergyMultiplier(passives, side, pair.Key);
            }

            return total;
        }

        public static float GetMatchEnergyMultiplier(
            IReadOnlyList<HeroPassiveRuntimeState> passives,
            BattleSide side,
            TileKind tileKind)
        {
            var percentBonus = 0;
            if (null != passives)
            {
                for (var i = 0; i < passives.Count; i++)
                {
                    var passive = passives[i];
                    if (false == IsMatchEnergyPassive(passive, side, tileKind))
                        continue;

                    percentBonus += passive.Definition.Power * passive.ActivationCount;
                }
            }

            return 1f + percentBonus / 100f;
        }

        private static bool IsMatchEnergyPassive(
            HeroPassiveRuntimeState passive,
            BattleSide side,
            TileKind tileKind)
        {
            return passive.Side == side
                   && passive is { IsActive: true, IsDisabled: false }
                   && passive.SlotKind == tileKind
                   && passive.Definition.AbilityKind == PassiveAbilityKind.MatchEnergyBySlotKindPercent;
        }
    }
}