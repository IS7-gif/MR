using System;
using R3;

namespace Project.Scripts.Services.Board
{
    public class BoardRuntimeService : IBoardRuntimeService, IDisposable
    {
        public ReadOnlyReactiveProperty<BoardRuntimeState> State => _state;
        public int CurrentVersion => _currentVersion;
        public bool IsRunning => _state.Value == BoardRuntimeState.Running;
        public bool IsStoppingForOvertime => _state.Value == BoardRuntimeState.StoppingForOvertime;
        public bool IsFrozen => _state.Value == BoardRuntimeState.Frozen;
        public bool CanAcceptInput => _state.Value == BoardRuntimeState.Running;


        private readonly ReactiveProperty<BoardRuntimeState> _state = new(BoardRuntimeState.Running);
        private int _currentVersion;


        public int CaptureVersion()
        {
            return _currentVersion;
        }

        public bool IsCurrent(int version)
        {
            return version == _currentVersion;
        }

        public void RequestOvertimeStop()
        {
            if (false == IsRunning)
                return;

            _currentVersion++;
            _state.Value = BoardRuntimeState.StoppingForOvertime;
        }

        public void MarkFrozen()
        {
            if (IsFrozen)
                return;

            if (IsRunning)
                _currentVersion++;

            _state.Value = BoardRuntimeState.Frozen;
        }

        public void Dispose()
        {
            _state.Dispose();
        }
    }
}