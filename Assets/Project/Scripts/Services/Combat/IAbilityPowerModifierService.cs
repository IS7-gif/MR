using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Services.Combat
{
    public interface IAbilityPowerModifierService
    {
        int GetAbilityPower(UnitDescriptor target, int basePower);
    }
}