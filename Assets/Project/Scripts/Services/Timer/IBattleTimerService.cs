namespace Project.Scripts.Services.Timer
{
    public interface IBattleTimerService
    {
        bool IsRunning { get; }
        void Initialize();
        void Tick(float deltaTime);
    }
}