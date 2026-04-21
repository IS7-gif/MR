namespace Project.Scripts.Services.Combat
{
    public interface IBattleEconomyModifierService
    {
        float CascadeEnergyMultiplier { get; }
        float AutoEnergyIntervalMultiplier { get; }
    }
}