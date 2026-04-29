using System.Collections.Generic;
using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Shared.Passives
{
    public static class PendingAttackBonusRules
    {
        public static void Grant(IDictionary<BattleUnitKey, int> bonuses, IReadOnlyList<UnitDescriptor> targets, int amount)
        {
            if (bonuses == null || targets == null || amount == 0)
                return;

            for (var i = 0; i < targets.Count; i++)
            {
                var key = BattleUnitKey.FromDescriptor(targets[i]);
                bonuses.TryGetValue(key, out var current);
                bonuses[key] = current + amount;
            }
        }

        public static int Consume(IDictionary<BattleUnitKey, int> bonuses, UnitDescriptor source)
        {
            if (bonuses == null)
                return 0;

            var key = BattleUnitKey.FromDescriptor(source);
            if (false == bonuses.TryGetValue(key, out var amount) || amount == 0)
                return 0;

            bonuses.Remove(key);
            
            return amount;
        }
    }
}