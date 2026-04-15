namespace Project.Scripts.Services.Combat
{
    public interface IEscalationModifierService
    {
        bool IsEscalationActive { get; }
        float CascadeEnergyMultiplier { get; }
        float AutoEnergyIntervalMultiplier { get; }
    }
}