using Project.Scripts.Shared.GroupDefense;
using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Services.Combat
{
    public interface IAvatarGroupDefenseService
    {
        bool IsExposed(BattleSide side);
        AvatarDefenseSnapshot GetSnapshot(BattleSide side);
    }
}