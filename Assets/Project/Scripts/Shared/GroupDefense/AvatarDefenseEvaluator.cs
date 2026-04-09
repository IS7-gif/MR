using System.Collections.Generic;
using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Shared.GroupDefense
{
    public static class AvatarDefenseEvaluator
    {
        public static bool IsGroupDestroyed(IReadOnlyList<HeroSlotState> slots, int[] groupSlotIndices)
        {
            for (var i = 0; i < groupSlotIndices.Length; i++)
            {
                var idx = groupSlotIndices[i];
                if (idx < 0 || idx >= slots.Count)
                    continue;

                var slot = slots[idx];
                if (slot.IsAssigned && slot.IsAlive)
                    return false;
            }

            return true;
        }

        public static AvatarDefenseSnapshot Evaluate(IReadOnlyList<HeroSlotState> slots, int[] group1SlotIndices,
            int[] group2SlotIndices)
        {
            var g1 = IsGroupDestroyed(slots, group1SlotIndices);
            var g2 = IsGroupDestroyed(slots, group2SlotIndices);
            
            return new AvatarDefenseSnapshot(g1, g2);
        }
    }
}