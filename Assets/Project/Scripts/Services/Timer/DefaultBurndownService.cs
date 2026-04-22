namespace Project.Scripts.Services.Timer
{
    public class DefaultBurndownService : IBurndownService
    {
        public bool IsActive => false;


        public void Begin()
        {
        }

        public void Tick(float deltaTime)
        {
        }
    }
}