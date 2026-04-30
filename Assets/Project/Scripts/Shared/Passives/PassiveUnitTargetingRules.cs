using System.Collections.Generic;
using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Shared.Passives
{
    public static class PassiveUnitTargetingRules
    {
        public static List<UnitDescriptor> SelectTargets(PassiveUnitTargetDefinition target, UnitDescriptor owner,
            IReadOnlyList<PassiveUnitTargetCandidate> candidates)
        {
            var result = new List<UnitDescriptor>();
            if (false == target.IsConfigured || target.SelectionMode != PassiveUnitSelectionMode.All || candidates == null)
                return result;

            for (var i = 0; i < candidates.Count; i++)
            {
                var candidate = candidates[i];
                if (false == candidate.IsAvailable)
                    continue;

                if (false == IsTeamMatch(target.Team, owner.Side, candidate.Descriptor.Side))
                    continue;

                if (false == IsKindMatch(target.UnitKind, candidate.Descriptor.Kind))
                    continue;

                if (target.ExcludeOwner && BattleUnitKey.FromDescriptor(candidate.Descriptor) == BattleUnitKey.FromDescriptor(owner))
                    continue;

                result.Add(candidate.Descriptor);
            }

            return result;
        }

        private static bool IsTeamMatch(PassiveUnitTargetTeam team, BattleSide ownerSide, BattleSide candidateSide)
        {
            if (team == PassiveUnitTargetTeam.Allies)
                return candidateSide == ownerSide;

            return false;
        }

        private static bool IsKindMatch(PassiveUnitTargetKind targetKind, UnitKind candidateKind)
        {
            if (targetKind == PassiveUnitTargetKind.Units)
                return true;

            return false;
        }
    }
}