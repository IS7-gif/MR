namespace Project.Scripts.Services.Combat
{
    public interface IPlayerAvatarChargeService
    {
        int CurrentCharge { get; }
        int MaxCharge { get; }
        bool IsFull { get; }
    }
}