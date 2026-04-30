using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Shared.Rules
{
    public static class AbilityTargetRules
    {
        public static bool IsTargetValid(UnitDescriptor source, UnitDescriptor target,
            HeroActionType actionType, bool isSourceAlive, bool isTargetAlive, bool isTargetHpFull,
            bool isTargetExposed)
        {
            if (false == isSourceAlive || false == isTargetAlive)
                return false;

            if (actionType == HeroActionType.DealDamage)
                return CanDealDamage(source, target, isTargetExposed);

            if (actionType == HeroActionType.HealAlly)
                return CanHeal(source, target, isTargetHpFull);

            return false;
        }

        private static bool CanDealDamage(UnitDescriptor source, UnitDescriptor target, bool isTargetExposed)
        {
            if (target.Side == source.Side)
                return false;

            if (target.Kind == UnitKind.Avatar && false == isTargetExposed)
                return false;

            return true;
        }

        private static bool CanHeal(UnitDescriptor source, UnitDescriptor target, bool isTargetHpFull)
        {
            if (target.Side != source.Side)
                return false;

            if (source.Kind == target.Kind && source.Side == target.Side && source.SlotIndex == target.SlotIndex)
                return false;

            if (isTargetHpFull)
                return false;

            return true;
        }
    }
}