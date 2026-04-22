using System;
using Project.Scripts.Shared.BattleFlow;
using R3;

namespace Project.Scripts.Services.Board
{
    public class BoardRuntimeService : IBoardRuntimeService, IDisposable
    {
        public ReadOnlyReactiveProperty<BoardRuntimeState> State => _state;
        public ReadOnlyReactiveProperty<bool> IsResolvingState => _isResolving;
        public int CurrentVersion => _currentVersion;
        public bool IsRunning => CanContinueResolution;
        public bool IsStoppingForOvertime => _state.Value == BoardRuntimeState.StoppingForOvertime;
        public bool IsFrozen => _state.Value == BoardRuntimeState.Frozen;
        public bool CanAcceptInput => _state.Value == BoardRuntimeState.MatchPhase;
        public bool CanContinueResolution => _state.Value is BoardRuntimeState.MatchPhase or BoardRuntimeState.MatchClosing;
        public bool IsResolving => _isResolving.Value;


        private readonly ReactiveProperty<BoardRuntimeState> _state = new(BoardRuntimeState.MatchPhase);
        private readonly ReactiveProperty<bool> _isResolving = new(false);
        private int _currentVersion;


        public int CaptureVersion()
        {
            return _currentVersion;
        }

        public bool IsCurrent(int version)
        {
            return version == _currentVersion;
        }

        public void ApplyBattleFlowPhase(BattlePhaseKind phase)
        {
            if (IsFrozen || IsStoppingForOvertime)
                return;

            var nextState = phase == BattlePhaseKind.Match
                ? BoardRuntimeState.MatchPhase
                : BoardRuntimeState.HeroPhaseSuspended;

            if (_state.Value == nextState)
                return;

            _currentVersion++;
            _state.Value = nextState;
        }

        public void RequestMatchPhaseClose()
        {
            if (IsFrozen || IsStoppingForOvertime)
                return;

            if (_state.Value != BoardRuntimeState.MatchPhase)
                return;

            _state.Value = BoardRuntimeState.MatchClosing;
        }

        public void BeginResolution()
        {
            if (false == _isResolving.Value)
                _isResolving.Value = true;
        }

        public void EndResolution()
        {
            if (_isResolving.Value)
                _isResolving.Value = false;
        }

        public void RequestOvertimeStop()
        {
            if (IsFrozen || IsStoppingForOvertime)
                return;

            _currentVersion++;
            _state.Value = BoardRuntimeState.StoppingForOvertime;
            _isResolving.Value = false;
        }

        public void MarkFrozen()
        {
            if (IsFrozen)
                return;

            if (_state.Value != BoardRuntimeState.Frozen)
                _currentVersion++;

            _state.Value = BoardRuntimeState.Frozen;
            _isResolving.Value = false;
        }

        public void Dispose()
        {
            _state.Dispose();
            _isResolving.Dispose();
        }
    }
}