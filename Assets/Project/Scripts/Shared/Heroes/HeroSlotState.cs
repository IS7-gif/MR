using Project.Scripts.Shared.Tiles;

namespace Project.Scripts.Shared.Heroes
{
    public struct HeroSlotState
    {
        public TileKind SlotKind;
        public bool IsAssigned;
        public float CurrentEnergy;
        public int MaxEnergy;
        public HeroActionType ActionType;
        public int ActionValue;
        public int CurrentHP;
        public int MaxHP;

        
        public bool IsAlive => MaxHP <= 0 || CurrentHP > 0;
        public bool CanAccumulateEnergy => IsAssigned && IsAlive;
        public bool IsReady => CanAccumulateEnergy && MaxEnergy > 0 && CurrentEnergy >= MaxEnergy;
    }
}