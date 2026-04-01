namespace Project.Scripts.Services.Combat
{
    public interface IEnemyAvatarChargeService
    {
        int CurrentCharge { get; }
        int MaxCharge { get; }
        bool IsFull { get; }

        //Called by BotOpponentService (and future: server message handler) to accumulate charge.
        void AddCharge(int amount);

        //Called by BotOpponentService (and future: server message handler) to discharge and deal damage.
        void TriggerDischarge();
    }
}