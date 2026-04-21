using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Services.Combat
{
    public interface IEnemyAvatarChargeService
    {
        int ActivationEnergyCost { get; }
        bool IsReady { get; }
        HeroActionType AbilityType { get; }
        int AbilityPower { get; }
        bool TryRelease();
        void AddEnergy(int amount);
        void TriggerAttack();
    }
}