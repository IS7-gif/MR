using System;
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
        public int ConditionCount => _conditionProgress?.Length ?? 0;

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
            int[] conditionProgress = null)
        {
            Side = side;
            SlotIndex = slotIndex;
            SlotKind = slotKind;
            Definition = definition;
            IsActive = isActive;
            IsDisabled = isDisabled;
            ActivationCount = activationCount < 0 ? 0 : activationCount;
            _conditionProgress = NormalizeProgress(definition, conditionProgress);
        }

        public int GetConditionProgress(int conditionIndex)
        {
            if (_conditionProgress == null || conditionIndex < 0 || conditionIndex >= _conditionProgress.Length)
                return 0;

            return _conditionProgress[conditionIndex];
        }

        public HeroPassiveRuntimeState WithConditionProgress(int conditionIndex, int progress)
        {
            var nextProgress = CopyProgress();
            if (conditionIndex >= 0 && conditionIndex < nextProgress.Length)
                nextProgress[conditionIndex] = progress < 0 ? 0 : progress;

            return new HeroPassiveRuntimeState(
                Side,
                SlotIndex,
                SlotKind,
                Definition,
                IsActive,
                IsDisabled,
                ActivationCount,
                nextProgress);
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
                CreateProgressArray(Definition));
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
                CopyProgress());
        }

        public HeroPassiveRuntimeState WithConditionProgressReset(int conditionIndex)
        {
            return WithConditionProgress(conditionIndex, 0);
        }


        private readonly int[] _conditionProgress;


        private int[] CopyProgress()
        {
            if (_conditionProgress == null || _conditionProgress.Length == 0)
                return Array.Empty<int>();

            var result = new int[_conditionProgress.Length];
            Array.Copy(_conditionProgress, result, _conditionProgress.Length);
            return result;
        }

        private static int[] NormalizeProgress(HeroPassiveDefinition definition, int[] progress)
        {
            var result = CreateProgressArray(definition);
            if (progress == null || progress.Length == 0)
                return result;

            var count = progress.Length < result.Length ? progress.Length : result.Length;
            for (var i = 0; i < count; i++)
                result[i] = progress[i] < 0 ? 0 : progress[i];

            return result;
        }

        private static int[] CreateProgressArray(HeroPassiveDefinition definition)
        {
            var conditions = definition.ConditionGroup.Conditions;
            return conditions.Count == 0 ? Array.Empty<int>() : new int[conditions.Count];
        }
    }
}