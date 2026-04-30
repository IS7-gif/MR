using System.Collections.Generic;
using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Shared.Passives
{
    public static class NextAttackBuffRules
    {
        public static void Grant(IDictionary<BattleUnitKey, int> buffs, IReadOnlyList<UnitDescriptor> targets, int amount)
        {
            if (buffs == null || targets == null || amount == 0)
                return;

            for (var i = 0; i < targets.Count; i++)
            {
                var key = BattleUnitKey.FromDescriptor(targets[i]);
                buffs.TryGetValue(key, out var current);
                buffs[key] = current + amount;
            }
        }

        public static int Consume(IDictionary<BattleUnitKey, int> buffs, UnitDescriptor source)
        {
            if (buffs == null)
                return 0;

            var key = BattleUnitKey.FromDescriptor(source);
            if (false == buffs.TryGetValue(key, out var amount) || amount == 0)
                return 0;

            buffs.Remove(key);

            return amount;
        }
    }
}
