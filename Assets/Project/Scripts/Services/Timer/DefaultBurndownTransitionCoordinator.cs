namespace Project.Scripts.Services.Timer
{
    public class DefaultBurndownTransitionCoordinator : IBurndownTransitionCoordinator
    {
        public bool IsStarted => false;


        public void RequestStart()
        {
        }
    }
}