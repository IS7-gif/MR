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

        public bool AddTriggerProgress(
            PassiveTriggerKind triggerKind,
            BattleSide side,
            int slotIndex,
            int currentRound,
            int amount = 1,
            TileKind slotKind = TileKind.None)
        {
            if (amount <= 0 || triggerKind == PassiveTriggerKind.None)
                return false;

            var changed = false;
            for (var i = 0; i < _states.Length; i++)
            {
                var state = _states[i];
                if (state.Side != side || false == state.CanActivateAgain)
                    continue;

                if (state.Definition.TriggerKind != triggerKind)
                    continue;

                if (slotIndex >= 0 && state.SlotIndex != slotIndex)
                    continue;

                if (slotKind != TileKind.None && state.SlotKind != slotKind)
                    continue;

                var nextState = state.WithTriggerProgress(state.TriggerProgress + amount);
                _states[i] = nextState.TriggerProgress >= nextState.Definition.RequiredTriggerCount
                    ? nextState.WithActivated(currentRound)
                    : nextState;
                changed = true;
            }

            return changed;
        }

        public bool ResetTriggerProgress(PassiveTriggerKind triggerKind, BattleSide side)
        {
            if (triggerKind == PassiveTriggerKind.None)
                return false;

            var changed = false;
            for (var i = 0; i < _states.Length; i++)
            {
                var state = _states[i];
                if (state.Side != side)
                    continue;

                if (state.Definition.TriggerKind != triggerKind || state.TriggerProgress == 0)
                    continue;

                _states[i] = state.WithTriggerProgress(0);
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

        public bool ExpireRoundLimitedPassives(int currentRound)
        {
            var changed = false;
            for (var i = 0; i < _states.Length; i++)
            {
                var state = _states[i];
                if (state.IsDisabled || false == state.IsActive || state.ExpiresAtRound <= 0)
                    continue;

                if (currentRound < state.ExpiresAtRound)
                    continue;

                _states[i] = state.WithExpired();
                changed = true;
            }

            return changed;
        }

    }
}