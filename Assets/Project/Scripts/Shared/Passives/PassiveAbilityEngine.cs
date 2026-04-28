using System;
using System.Collections.Generic;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Tiles;

namespace Project.Scripts.Shared.Passives
{
    public class PassiveAbilityEngine
    {
        public IReadOnlyList<HeroPassiveRuntimeState> States => _states;


        private HeroPassiveRuntimeState[] _states = Array.Empty<HeroPassiveRuntimeState>();

        
        public void Initialize(IReadOnlyList<HeroPassiveSetup> setups)
        {
            if (setups == null || setups.Count == 0)
            {
                _states = Array.Empty<HeroPassiveRuntimeState>();
                return;
            }

            var states = new List<HeroPassiveRuntimeState>(setups.Count);
            for (var i = 0; i < setups.Count; i++)
            {
                var setup = setups[i];
                if (false == setup.Definition.IsConfigured)
                    continue;

                states.Add(new HeroPassiveRuntimeState(
                    setup.Side,
                    setup.SlotIndex,
                    setup.SlotKind,
                    setup.Definition));
            }

            _states = states.ToArray();
        }

        public bool AddConditionProgress(
            PassiveConditionKind conditionKind,
            BattleSide side,
            int amount = 1,
            int slotIndex = -1,
            TileKind slotKind = TileKind.None)
        {
            if (amount <= 0 || conditionKind == PassiveConditionKind.None)
                return false;

            var changed = false;
            for (var i = 0; i < _states.Length; i++)
            {
                var state = _states[i];
                if (state.Side != side || false == state.CanActivateAgain)
                    continue;

                var nextState = AddProgressToMatchingConditions(state, conditionKind, amount, slotIndex, slotKind);
                if (false == HasSameConditionProgress(state, nextState))
                {
                    _states[i] = IsConditionGroupSatisfied(nextState)
                        ? nextState.WithActivated()
                        : nextState;
                    changed = true;
                }
            }

            return changed;
        }

        public bool ResetConditionProgress(PassiveConditionKind conditionKind, BattleSide side)
        {
            if (conditionKind == PassiveConditionKind.None)
                return false;

            var changed = false;
            for (var i = 0; i < _states.Length; i++)
            {
                var state = _states[i];
                if (state.Side != side)
                    continue;

                var nextState = ResetMatchingConditions(state, conditionKind);
                if (HasSameConditionProgress(state, nextState))
                    continue;

                _states[i] = nextState;
                changed = true;
            }

            return changed;
        }

        public bool DisableOwner(BattleSide side, int slotIndex)
        {
            var changed = false;
            for (var i = 0; i < _states.Length; i++)
            {
                var state = _states[i];
                if (state.Side != side || state.SlotIndex != slotIndex || state.IsDisabled)
                    continue;

                _states[i] = state.WithDisabled();
                changed = true;
            }

            return changed;
        }


        private static HeroPassiveRuntimeState AddProgressToMatchingConditions(
            HeroPassiveRuntimeState state,
            PassiveConditionKind conditionKind,
            int amount,
            int slotIndex,
            TileKind slotKind)
        {
            if (slotIndex >= 0 && state.SlotIndex != slotIndex)
                return state;

            if (slotKind != TileKind.None && state.SlotKind != slotKind)
                return state;

            var result = state;
            var conditions = state.Definition.ConditionGroup.Conditions;
            for (var i = 0; i < conditions.Count; i++)
            {
                if (conditions[i].Kind != conditionKind)
                    continue;

                result = result.WithConditionProgress(i, result.GetConditionProgress(i) + amount);
            }

            return result;
        }

        private static HeroPassiveRuntimeState ResetMatchingConditions(
            HeroPassiveRuntimeState state,
            PassiveConditionKind conditionKind)
        {
            var result = state;
            var conditions = state.Definition.ConditionGroup.Conditions;
            for (var i = 0; i < conditions.Count; i++)
            {
                if (conditions[i].Kind != conditionKind || result.GetConditionProgress(i) == 0)
                    continue;

                result = result.WithConditionProgressReset(i);
            }

            return result;
        }

        private static bool IsConditionGroupSatisfied(HeroPassiveRuntimeState state)
        {
            var conditions = state.Definition.ConditionGroup.Conditions;
            if (conditions.Count == 0)
                return false;

            if (state.Definition.ConditionGroup.Operator == PassiveConditionGroupOperator.Or)
            {
                for (var i = 0; i < conditions.Count; i++)
                    if (state.GetConditionProgress(i) >= conditions[i].RequiredValue)
                        return true;

                return false;
            }

            for (var i = 0; i < conditions.Count; i++)
                if (state.GetConditionProgress(i) < conditions[i].RequiredValue)
                    return false;

            return true;
        }

        private static bool HasSameConditionProgress(HeroPassiveRuntimeState left, HeroPassiveRuntimeState right)
        {
            if (left.ConditionCount != right.ConditionCount)
                return false;

            for (var i = 0; i < left.ConditionCount; i++)
                if (left.GetConditionProgress(i) != right.GetConditionProgress(i))
                    return false;

            return true;
        }
    }
}