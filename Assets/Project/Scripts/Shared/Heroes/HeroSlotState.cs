using Project.Scripts.Shared.Tiles;

namespace Project.Scripts.Shared.Heroes
{
    public struct HeroSlotState
    {
        public bool IsAssigned;
        public TileKind Kind;
        public int CurrentEnergy;
        public int MaxEnergy;
        public HeroActionType ActionType;
        public int ActionValue;


        public bool IsReady => IsAssigned && MaxEnergy > 0 && CurrentEnergy >= MaxEnergy;
    }
}