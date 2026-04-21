using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Services.Combat
{
    public interface IPlayerAvatarChargeService
    {
        int ActivationEnergyCost { get; }
        bool IsReady { get; }
        HeroActionType AbilityType { get; }
        int AbilityPower { get; }
        bool TryRelease();
    }
}
