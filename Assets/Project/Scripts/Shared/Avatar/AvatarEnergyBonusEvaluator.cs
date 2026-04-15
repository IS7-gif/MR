using System.Collections.Generic;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Tiles;

namespace Project.Scripts.Shared.Avatar
{
    public static class AvatarEnergyBonusEvaluator
    {
        public static void CollectBonusKinds(HeroSlotState[] slots, ICollection<TileKind> result)
        {
            for (var i = 0; i < slots.Length; i++)
            {
                var slot = slots[i];
                if (slot is { IsAssigned: true, IsAlive: false })
                    result.Add(slot.SlotKind);
            }
        }
    }
}