using System.Collections.Generic;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Passives;

namespace Project.Scripts.Services.Combat
{
    public class NextAttackBuffService : INextAttackBuffService
    {
        private readonly Dictionary<BattleUnitKey, int> _nextAttackBuffs = new();


        public int Get(UnitDescriptor source)
        {
            return _nextAttackBuffs.GetValueOrDefault(BattleUnitKey.FromDescriptor(source), 0);
        }

        public int Consume(UnitDescriptor source)
        {
            return NextAttackBuffRules.Consume(_nextAttackBuffs, source);
        }

        public void Grant(IReadOnlyList<UnitDescriptor> targets, int amount)
        {
            NextAttackBuffRules.Grant(_nextAttackBuffs, targets, amount);
        }
    }
}
