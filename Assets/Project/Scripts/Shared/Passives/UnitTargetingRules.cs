using System.Collections.Generic;
using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Shared.Passives
{
    public static class UnitTargetingRules
    {
        public static List<UnitDescriptor> SelectTargets(UnitTargetingDefinition targeting, UnitDescriptor owner,
            IReadOnlyList<UnitTargetCandidate> candidates)
        {
            if (targeting.Scope == UnitTargetScope.Self)
                return new List<UnitDescriptor> { owner };

            var result = new List<UnitDescriptor>();
            if (targeting.SelectionMode != UnitTargetSelectionMode.All || candidates == null)
                return result;

            for (var i = 0; i < candidates.Count; i++)
            {
                var candidate = candidates[i];
                if (false == candidate.IsAvailable)
                    continue;

                if (false == targeting.IncludeOwner && BattleUnitKey.FromDescriptor(candidate.Descriptor) == BattleUnitKey.FromDescriptor(owner))
                    continue;

                if (false == IsRelationMatch(targeting.Relation, owner.Side, candidate.Descriptor.Side))
                    continue;

                if (false == IsKindMatch(targeting.UnitKind, candidate.Descriptor.Kind))
                    continue;

                if (false == PassesFilters(targeting.Filters, candidate))
                    continue;

                result.Add(candidate.Descriptor);
            }

            return result;
        }

        private static bool IsRelationMatch(UnitTargetRelation relation, BattleSide ownerSide, BattleSide candidateSide)
        {
            if (relation == UnitTargetRelation.Everyone)
                return true;

            if (relation == UnitTargetRelation.Allies)
                return candidateSide == ownerSide;

            if (relation == UnitTargetRelation.Enemies)
                return candidateSide != ownerSide;

            return false;
        }

        private static bool IsKindMatch(UnitTargetKind targetKind, UnitKind candidateKind)
        {
            if (targetKind == UnitTargetKind.Units)
                return true;

            if (targetKind == UnitTargetKind.Heroes)
                return candidateKind == UnitKind.Hero;

            if (targetKind == UnitTargetKind.Avatar)
                return candidateKind == UnitKind.Avatar;

            return false;
        }

        private static bool PassesFilters(IReadOnlyList<UnitTargetFilter> filters, UnitTargetCandidate candidate)
        {
            if (null == filters || filters.Count == 0)
                return true;

            for (var i = 0; i < filters.Count; i++)
                if (filters[i] == UnitTargetFilter.CanDealDamage && candidate.Descriptor.ActionType != HeroActionType.DealDamage)
                    return false;

            return true;
        }
    }
}