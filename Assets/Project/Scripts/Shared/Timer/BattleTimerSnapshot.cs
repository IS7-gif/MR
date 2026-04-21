namespace Project.Scripts.Shared.Timer
{
    public readonly struct BattleTimerSnapshot
    {
        public float TimeRemaining { get; }


        public BattleTimerSnapshot(float timeRemaining)
        {
            TimeRemaining = timeRemaining;
        }
    }
}