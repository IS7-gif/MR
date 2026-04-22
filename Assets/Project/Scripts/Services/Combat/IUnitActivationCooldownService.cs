using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Services.Combat
{
    public interface IUnitActivationCooldownService
    {
        void Tick(float deltaTime);
        bool IsHeroOnCooldown(BattleSide side, int slotIndex);
        bool IsAvatarOnCooldown(BattleSide side);
        void StartHeroCooldown(BattleSide side, int slotIndex);
        void StartAvatarCooldown(BattleSide side);
    }
}