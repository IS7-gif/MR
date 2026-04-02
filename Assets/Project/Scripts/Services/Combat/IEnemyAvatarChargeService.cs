namespace Project.Scripts.Services.Combat
{
    public interface IEnemyAvatarChargeService
    {
        int CurrentEnergy { get; }
        int MaxEnergy { get; }
        bool IsReady { get; }
        void AddEnergy(int amount);
        void TriggerAttack();
    }
}