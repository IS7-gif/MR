namespace Project.Scripts.Services.Timer
{
    public interface IOvertimeTransitionCoordinator
    {
        bool IsStarted { get; }
        void RequestStart();
    }
}