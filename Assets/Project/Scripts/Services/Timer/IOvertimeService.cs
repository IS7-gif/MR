namespace Project.Scripts.Services.Timer
{
    public interface IOvertimeService
    {
        bool IsActive { get; }
        void Begin();
        void Tick(float deltaTime);
    }
}