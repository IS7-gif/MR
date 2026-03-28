using System.Collections.Generic;
using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Services.Combat
{
    public interface IHeroService
    {
        IReadOnlyList<HeroSlotState> GetSlots(BattleSide side);
        void TryActivate(int slotIndex);
    }
}