using System;
using Project.Scripts.Shared.BattleFlow;
using Project.Scripts.Shared.Rules;
using R3;

namespace Project.Scripts.Services.Game
{
    public class BattleActionRuntimeService : IBattleActionRuntimeService, IDisposable
    {
        public ReadOnlyReactiveProperty<BattleActionRuntimeState> State => _state;
        public int CurrentVersion => _currentVersion;
        public bool IsRunning => _state.Value is BattleActionRuntimeState.MatchPhaseBlocked or BattleActionRuntimeState.HeroPhase;
        public bool IsStoppingForOvertime => _state.Value == BattleActionRuntimeState.StoppingForOvertime;
        public bool IsBlocked => _state.Value == BattleActionRuntimeState.Blocked;
        public bool CanAcceptNormalActions => _state.Value == BattleActionRuntimeState.HeroPhase;
        public BattleActionPhase CurrentPhase => MapPhase(_state.Value);


        private readonly ReactiveProperty<BattleActionRuntimeState> _state = new(BattleActionRuntimeState.MatchPhaseBlocked);
        private int _currentVersion;


        public int CaptureVersion()
        {
            return _currentVersion;
        }

        public bool IsCurrent(int version)
        {
            return version == _currentVersion;
        }

        public BattleActionGateResult Evaluate(BattleActionKind actionKind)
        {
            return BattleActionGateRules.Evaluate(CurrentPhase, actionKind);
        }

        public void ApplyBattleFlowPhase(BattlePhaseKind phase)
        {
            if (IsBlocked || IsStoppingForOvertime)
                return;

            BattleActionRuntimeState nextState;
            switch (phase)
            {
                case BattlePhaseKind.Match:
                    nextState = BattleActionRuntimeState.MatchPhaseBlocked;
                    break;
                case BattlePhaseKind.Hero:
                    nextState = BattleActionRuntimeState.HeroPhase;
                    break;
                default:
                    nextState = BattleActionRuntimeState.Blocked;
                    break;
            }

            if (_state.Value == nextState)
                return;

            _currentVersion++;
            _state.Value = nextState;
        }

        public void RequestOvertimeStop()
        {
            if (IsBlocked || IsStoppingForOvertime)
                return;

            _currentVersion++;
            _state.Value = BattleActionRuntimeState.StoppingForOvertime;
        }

        public void MarkBlocked()
        {
            if (IsBlocked)
                return;

            if (IsRunning)
                _currentVersion++;

            _state.Value = BattleActionRuntimeState.Blocked;
        }

        public void Dispose()
        {
            _state.Dispose();
        }


        private static BattleActionPhase MapPhase(BattleActionRuntimeState state)
        {
            if (state == BattleActionRuntimeState.MatchPhaseBlocked)
                return BattleActionPhase.MatchPhase;

            if (state == BattleActionRuntimeState.HeroPhase)
                return BattleActionPhase.HeroPhase;

            if (state == BattleActionRuntimeState.StoppingForOvertime)
                return BattleActionPhase.Overtime;

            return BattleActionPhase.Finished;
        }
    }
}