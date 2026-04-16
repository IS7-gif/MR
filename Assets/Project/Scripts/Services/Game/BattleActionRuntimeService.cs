using System;
using Project.Scripts.Shared.Rules;
using R3;

namespace Project.Scripts.Services.Game
{
    public class BattleActionRuntimeService : IBattleActionRuntimeService, IDisposable
    {
        public ReadOnlyReactiveProperty<BattleActionRuntimeState> State => _state;
        public int CurrentVersion => _currentVersion;
        public bool IsRunning => _state.Value == BattleActionRuntimeState.Running;
        public bool IsStoppingForOvertime => _state.Value == BattleActionRuntimeState.StoppingForOvertime;
        public bool IsBlocked => _state.Value == BattleActionRuntimeState.Blocked;
        public bool CanAcceptNormalActions => _state.Value == BattleActionRuntimeState.Running;
        public BattleActionPhase CurrentPhase => MapPhase(_state.Value);


        private readonly ReactiveProperty<BattleActionRuntimeState> _state = new(BattleActionRuntimeState.Running);
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

        public void RequestOvertimeStop()
        {
            if (false == IsRunning)
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
            if (state == BattleActionRuntimeState.Running)
                return BattleActionPhase.NormalPlay;

            if (state == BattleActionRuntimeState.StoppingForOvertime || state == BattleActionRuntimeState.Blocked)
                return BattleActionPhase.Overtime;

            return BattleActionPhase.Finished;
        }
    }
}