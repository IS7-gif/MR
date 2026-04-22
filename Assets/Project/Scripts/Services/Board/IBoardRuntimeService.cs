using R3;
using Project.Scripts.Shared.BattleFlow;

namespace Project.Scripts.Services.Board
{
    public interface IBoardRuntimeService
    {
        ReadOnlyReactiveProperty<BoardRuntimeState> State { get; }
        ReadOnlyReactiveProperty<bool> IsResolvingState { get; }
        int CurrentVersion { get; }
        bool IsRunning { get; }
        bool IsStoppingForOvertime { get; }
        bool IsFrozen { get; }
        bool CanAcceptInput { get; }
        bool CanContinueResolution { get; }
        bool IsResolving { get; }
        int CaptureVersion();
        bool IsCurrent(int version);
        void ApplyBattleFlowPhase(BattlePhaseKind phase);
        void RequestMatchPhaseClose();
        void BeginResolution();
        void EndResolution();
        void RequestOvertimeStop();
        void MarkFrozen();
    }
}