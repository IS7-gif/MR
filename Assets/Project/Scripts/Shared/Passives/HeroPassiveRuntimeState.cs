using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Tiles;

namespace Project.Scripts.Shared.Passives
{
    public readonly struct HeroPassiveRuntimeState
    {
        public BattleSide Side { get; }
        public int SlotIndex { get; }
        public TileKind SlotKind { get; }
        public HeroPassiveDefinition Definition { get; }
        public bool IsActive { get; }
        public bool IsDisabled { get; }
        public int ActivationCount { get; }
        public int ConditionProgress { get; }

        public bool CanActivateAgain =>
            false == IsDisabled
            && Definition.IsConfigured
            && (false == IsActive || Definition.AllowMultipleActivations)
            && (Definition.ActivationLimit == 0 || ActivationCount < Definition.ActivationLimit);


        public HeroPassiveRuntimeState(
            BattleSide side,
            int slotIndex,
            TileKind slotKind,
            HeroPassiveDefinition definition,
            bool isActive = false,
            bool isDisabled = false,
            int activationCount = 0,
            int conditionProgress = 0)
        {
            Side = side;
            SlotIndex = slotIndex;
            SlotKind = slotKind;
            Definition = definition;
            IsActive = isActive;
            IsDisabled = isDisabled;
            ActivationCount = activationCount < 0 ? 0 : activationCount;
            ConditionProgress = conditionProgress < 0 ? 0 : conditionProgress;
        }

        public HeroPassiveRuntimeState WithProgress(int progress)
        {
            return new HeroPassiveRuntimeState(
                Side,
                SlotIndex,
                SlotKind,
                Definition,
                IsActive,
                IsDisabled,
                ActivationCount,
                progress);
        }

        public HeroPassiveRuntimeState WithActivated()
        {
            return new HeroPassiveRuntimeState(
                Side,
                SlotIndex,
                SlotKind,
                Definition,
                true,
                IsDisabled,
                ActivationCount + 1,
                0);
        }

        public HeroPassiveRuntimeState WithDisabled()
        {
            return new HeroPassiveRuntimeState(
                Side,
                SlotIndex,
                SlotKind,
                Definition,
                false,
                true,
                ActivationCount,
                ConditionProgress);
        }

        public HeroPassiveRuntimeState WithProgressReset()
        {
            return WithProgress(0);
        }
    }
}