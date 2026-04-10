namespace Project.Scripts.Shared.Timer
{
    public interface IBattleTimerEngine
    {
        BattleTimerSnapshot Snapshot { get; }
        void Initialize(float battleDuration);
        bool Tick(float deltaTime);
    }
}