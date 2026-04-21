using R3;
using Project.Scripts.Shared.BattleFlow;

namespace Project.Scripts.Services.Board
{
    public interface IBoardRuntimeService
    {
        ReadOnlyReactiveProperty<BoardRuntimeState> State { get; }
        int CurrentVersion { get; }
        bool IsRunning { get; }
        bool IsStoppingForOvertime { get; }
        bool IsFrozen { get; }
        bool CanAcceptInput { get; }
        int CaptureVersion();
        bool IsCurrent(int version);
        void ApplyBattleFlowPhase(BattlePhaseKind phase);
        void RequestOvertimeStop();
        void MarkFrozen();
    }
}