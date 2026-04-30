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
        public bool IsActive => ActiveStackCount > 0;
        public bool IsDisabled { get; }
        public int TotalActivationCount { get; }
        public int ActiveStackCount { get; }
        public int TriggerProgress { get; }
        public int ExpiresAtRound { get; }

        public bool CanActivateAgain =>
            false == IsDisabled
            && Definition.IsConfigured
            && (false == IsActive || Definition.CanActivateWhileActive)
            && (Definition.MaxActivations == 0 || TotalActivationCount < Definition.MaxActivations);


        public HeroPassiveRuntimeState(
            BattleSide side,
            int slotIndex,
            TileKind slotKind,
            HeroPassiveDefinition definition,
            bool isDisabled = false,
            int totalActivationCount = 0,
            int activeStackCount = 0,
            int triggerProgress = 0,
            int expiresAtRound = 0)
        {
            Side = side;
            SlotIndex = slotIndex;
            SlotKind = slotKind;
            Definition = definition;
            IsDisabled = isDisabled;
            TotalActivationCount = totalActivationCount < 0 ? 0 : totalActivationCount;
            ActiveStackCount = activeStackCount < 0 ? 0 : activeStackCount;
            TriggerProgress = triggerProgress < 0 ? 0 : triggerProgress;
            ExpiresAtRound = expiresAtRound < 0 ? 0 : expiresAtRound;
        }

        public HeroPassiveRuntimeState WithTriggerProgress(int progress)
        {
            return new HeroPassiveRuntimeState(
                Side,
                SlotIndex,
                SlotKind,
                Definition,
                IsDisabled,
                TotalActivationCount,
                ActiveStackCount,
                progress,
                ExpiresAtRound);
        }

        public HeroPassiveRuntimeState WithActivated(int currentRound)
        {
            var expiresAtRound = Definition.ActiveDurationRounds > 0
                ? currentRound + Definition.ActiveDurationRounds
                : 0;

            return new HeroPassiveRuntimeState(
                Side,
                SlotIndex,
                SlotKind,
                Definition,
                IsDisabled,
                TotalActivationCount + 1,
                ActiveStackCount + 1,
                0,
                expiresAtRound);
        }

        public HeroPassiveRuntimeState WithDisabled()
        {
            return new HeroPassiveRuntimeState(
                Side,
                SlotIndex,
                SlotKind,
                Definition,
                true,
                TotalActivationCount,
                0,
                TriggerProgress,
                0);
        }

        public HeroPassiveRuntimeState WithExpired()
        {
            return new HeroPassiveRuntimeState(
                Side,
                SlotIndex,
                SlotKind,
                Definition,
                IsDisabled,
                TotalActivationCount,
                0,
                TriggerProgress,
                0);
        }
    }
}