using System;
using System.Collections.Generic;
using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Shared.Passives
{
    public static class PassiveUnitTargetingRules
    {
        public static List<UnitDescriptor> SelectTargets(PassiveUnitTargetDefinition target, UnitDescriptor owner,
            IReadOnlyList<PassiveUnitTargetCandidate> candidates, Random random = null)
        {
            var filtered = CollectCandidates(target, owner, candidates);
            if (filtered.Count == 0)
                return filtered;

            if (target.SelectionMode == PassiveUnitSelectionMode.All)
                return filtered;

            if (target.SelectionMode == PassiveUnitSelectionMode.MostWounded)
                return new List<UnitDescriptor> { PickMostWounded(filtered, candidates) };

            if (target.SelectionMode == PassiveUnitSelectionMode.RandomOne)
                return PickRandom(filtered, 1, random);

            if (target.SelectionMode == PassiveUnitSelectionMode.RandomCount)
                return PickRandom(filtered, target.Count, random);

            return new List<UnitDescriptor>();
        }

        private static List<UnitDescriptor> CollectCandidates(PassiveUnitTargetDefinition target,
            UnitDescriptor owner, IReadOnlyList<PassiveUnitTargetCandidate> candidates)
        {
            var result = new List<UnitDescriptor>();
            if (false == target.IsConfigured || candidates == null)
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
            if (team == PassiveUnitTargetTeam.Both)
                return true;

            if (team == PassiveUnitTargetTeam.Allies)
                return candidateSide == ownerSide;

            if (team == PassiveUnitTargetTeam.Enemies)
                return candidateSide != ownerSide;

            return false;
        }

        private static bool IsKindMatch(PassiveUnitTargetKind targetKind, UnitKind candidateKind)
        {
            if (targetKind == PassiveUnitTargetKind.Units)
                return true;

            if (targetKind == PassiveUnitTargetKind.Heroes)
                return candidateKind == UnitKind.Hero;

            if (targetKind == PassiveUnitTargetKind.Avatar)
                return candidateKind == UnitKind.Avatar;

            return false;
        }

        private static UnitDescriptor PickMostWounded(
            IReadOnlyList<UnitDescriptor> descriptors,
            IReadOnlyList<PassiveUnitTargetCandidate> candidates)
        {
            var best = descriptors[0];
            var bestCurrent = 1;
            var bestMax = 1;
            TryGetHealth(best, candidates, out bestCurrent, out bestMax);

            for (var i = 1; i < descriptors.Count; i++)
            {
                var current = descriptors[i];
                TryGetHealth(current, candidates, out var currentHP, out var maxHP);
                if (IsMoreWounded(currentHP, maxHP, bestCurrent, bestMax)
                    || (HasSameHealthPercent(currentHP, maxHP, bestCurrent, bestMax)
                        && CompareStableOrder(current, best) < 0))
                {
                    best = current;
                    bestCurrent = currentHP;
                    bestMax = maxHP;
                }
            }

            return best;
        }

        private static bool TryGetHealth(UnitDescriptor descriptor, IReadOnlyList<PassiveUnitTargetCandidate> candidates,
            out int currentHP, out int maxHP)
        {
            for (var i = 0; i < candidates.Count; i++)
            {
                if (BattleUnitKey.FromDescriptor(candidates[i].Descriptor) != BattleUnitKey.FromDescriptor(descriptor))
                    continue;

                currentHP = candidates[i].CurrentHP;
                maxHP = candidates[i].MaxHP <= 0 ? 1 : candidates[i].MaxHP;
                return true;
            }

            currentHP = 1;
            maxHP = 1;
            
            return false;
        }

        private static bool IsMoreWounded(int leftCurrent, int leftMax, int rightCurrent, int rightMax)
        {
            return leftCurrent * rightMax < rightCurrent * leftMax;
        }

        private static bool HasSameHealthPercent(int leftCurrent, int leftMax, int rightCurrent, int rightMax)
        {
            return leftCurrent * rightMax == rightCurrent * leftMax;
        }

        private static int CompareStableOrder(UnitDescriptor left, UnitDescriptor right)
        {
            if (left.Side != right.Side)
                return left.Side == BattleSide.Player ? -1 : 1;

            if (left.Kind != right.Kind)
                return left.Kind == UnitKind.Avatar ? -1 : 1;

            return left.SlotIndex.CompareTo(right.SlotIndex);
        }

        private static List<UnitDescriptor> PickRandom(
            IReadOnlyList<UnitDescriptor> candidates,
            int count,
            Random random)
        {
            var result = new List<UnitDescriptor>();
            if (count <= 0)
                return result;

            var pool = new List<UnitDescriptor>(candidates);
            var rng = random ?? new Random();
            while (pool.Count > 0 && result.Count < count)
            {
                var index = rng.Next(pool.Count);
                result.Add(pool[index]);
                pool.RemoveAt(index);
            }

            return result;
        }
    }
}