using System.Collections.Generic;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Passives;

namespace Project.Scripts.Services.Combat
{
    public class PendingAttackBonusService : IPendingAttackBonusService
    {
        private readonly Dictionary<BattleUnitKey, int> _pendingAttackBonuses = new();


        public int Get(UnitDescriptor source)
        {
            return _pendingAttackBonuses.GetValueOrDefault(BattleUnitKey.FromDescriptor(source), 0);
        }

        public int Consume(UnitDescriptor source)
        {
            return PendingAttackBonusRules.Consume(_pendingAttackBonuses, source);
        }

        public void Grant(IReadOnlyList<UnitDescriptor> targets, int amount)
        {
            PendingAttackBonusRules.Grant(_pendingAttackBonuses, targets, amount);
        }
    }
}