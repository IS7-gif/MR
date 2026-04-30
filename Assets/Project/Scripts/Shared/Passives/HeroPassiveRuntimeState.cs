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
        public bool IsDisabled { get; }
        public int TotalActivationCount { get; }
        public int ConditionCount => _conditionProgress?.Length ?? 0;

        public bool CanActivateAgain =>
            false == IsDisabled
            && Definition.IsConfigured
            && (Definition.MaxActivations == 0 || TotalActivationCount < Definition.MaxActivations);


        private readonly int[] _conditionProgress;


        public HeroPassiveRuntimeState(BattleSide side, int slotIndex, TileKind slotKind, 
            HeroPassiveDefinition definition, bool isDisabled = false, 
            int totalActivationCount = 0, int[] conditionProgress = null)
        {
            Side = side;
            SlotIndex = slotIndex;
            SlotKind = slotKind;
            Definition = definition;
            IsDisabled = isDisabled;
            TotalActivationCount = totalActivationCount < 0 ? 0 : totalActivationCount;
            _conditionProgress = CopyConditionProgress(definition, conditionProgress);
        }

        public int GetConditionProgress(int conditionIndex)
        {
            return null != _conditionProgress && conditionIndex >= 0 && conditionIndex < _conditionProgress.Length
                ? _conditionProgress[conditionIndex]
                : 0;
        }

        public HeroPassiveRuntimeState WithConditionProgress(int conditionIndex, int progress)
        {
            if (null == _conditionProgress || conditionIndex < 0 || conditionIndex >= _conditionProgress.Length)
                return this;

            var nextProgress = CopyConditionProgress(Definition, _conditionProgress);
            nextProgress[conditionIndex] = progress < 0 ? 0 : progress;

            return new HeroPassiveRuntimeState(Side, SlotIndex, SlotKind, Definition, IsDisabled,
                TotalActivationCount, nextProgress);
        }

        public HeroPassiveRuntimeState WithActivated()
        {
            return new HeroPassiveRuntimeState(Side, SlotIndex, SlotKind, Definition, IsDisabled,
                TotalActivationCount + 1);
        }

        public HeroPassiveRuntimeState WithDisabled()
        {
            return new HeroPassiveRuntimeState(Side, SlotIndex, SlotKind, Definition, true,
                TotalActivationCount, _conditionProgress);
        }

        private static int[] CopyConditionProgress(HeroPassiveDefinition definition, int[] source)
        {
            var conditions = definition.ActivationConditions.Conditions;
            if (conditions.Count == 0)
                return System.Array.Empty<int>();

            var result = new int[conditions.Count];
            if (null == source)
                return result;

            for (var i = 0; i < result.Length && i < source.Length; i++)
                result[i] = source[i] < 0 ? 0 : source[i];

            return result;
        }
    }
}