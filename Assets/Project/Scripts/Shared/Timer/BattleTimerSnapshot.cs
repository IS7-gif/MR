namespace Project.Scripts.Shared.Timer
{
    public readonly struct BattleTimerSnapshot
    {
        public float TimeRemaining { get; }
        public bool IsOvertime { get; }


        public BattleTimerSnapshot(float timeRemaining, bool isOvertime)
        {
            TimeRemaining = timeRemaining;
            IsOvertime = isOvertime;
        }
    }
}