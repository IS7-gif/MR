using Project.Scripts.Shared.BattleFlow;

namespace Project.Scripts.Services.BattleFlow
{
    public interface IBattleFlowService
    {
        bool IsInitialized { get; }
        BattleFlowSnapshot Snapshot { get; }
        void Initialize();
        void Tick(float deltaTime);
        void BeginHeroPhase();
        void MarkFinished();
    }
}