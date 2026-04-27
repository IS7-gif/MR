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
                if (false == CanReceiveProgress(state, conditionKind, side, slotIndex, slotKind))
                    continue;

                var nextProgress = state.ConditionProgress + amount;
                _states[i] = nextProgress >= state.Definition.ConditionRequirement
                    ? state.WithActivated()
                    : state.WithProgress(nextProgress);

                changed = true;
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
                if (state.Side != side || state.Definition.ConditionKind != conditionKind || state.ConditionProgress == 0)
                    continue;

                _states[i] = state.WithProgressReset();
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


        private static bool CanReceiveProgress(
            HeroPassiveRuntimeState state,
            PassiveConditionKind conditionKind,
            BattleSide side,
            int slotIndex,
            TileKind slotKind)
        {
            if (state.Side != side || state.Definition.ConditionKind != conditionKind)
                return false;

            if (slotIndex >= 0 && state.SlotIndex != slotIndex)
                return false;

            if (slotKind != TileKind.None && state.SlotKind != slotKind)
                return false;

            return state.CanActivateAgain;
        }
    }
}