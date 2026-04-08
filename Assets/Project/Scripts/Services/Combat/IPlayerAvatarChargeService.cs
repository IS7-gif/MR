using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Services.Combat
{
    public interface IPlayerAvatarChargeService
    {
        int CurrentEnergy { get; }
        int MaxEnergy { get; }
        bool IsReady { get; }
        HeroActionType AbilityType { get; }
        int AbilityPower { get; }
        bool TryRelease();
    }
}
