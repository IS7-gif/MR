namespace Project.Scripts.Services.Progression
{
    public interface ILevelProgressionService
    {
        void Advance();
        void Retry();
    }
}