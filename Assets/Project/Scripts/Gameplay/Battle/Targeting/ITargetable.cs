using Project.Scripts.Shared.Heroes;
using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.Targeting
{
    public interface ITargetable
    {
        UnitDescriptor Descriptor { get; }
        bool IsReadySource { get; }
        Bounds WorldBounds { get; }

        bool IsValidTarget(UnitDescriptor source);
        void SetSourceHighlight(bool active);
        void SetTargetHighlight(bool active, HeroActionType actionType);
    }
}
