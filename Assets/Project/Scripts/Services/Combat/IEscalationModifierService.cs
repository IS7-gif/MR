namespace Project.Scripts.Services.Combat
{
    public interface IEscalationModifierService : IBattleEconomyModifierService
    {
        bool IsEscalationActive { get; }
        void ActivateEscalation();
    }
}