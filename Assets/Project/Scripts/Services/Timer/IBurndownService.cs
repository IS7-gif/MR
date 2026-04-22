namespace Project.Scripts.Services.Timer
{
    public interface IBurndownService
    {
        bool IsActive { get; }
        void Begin();
        void Tick(float deltaTime);
    }
}