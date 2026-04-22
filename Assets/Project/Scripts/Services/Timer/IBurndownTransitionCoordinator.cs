namespace Project.Scripts.Services.Timer
{
    public interface IBurndownTransitionCoordinator
    {
        bool IsStarted { get; }
        void RequestStart();
    }
}