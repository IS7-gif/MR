namespace Project.Scripts.Shared.BattleFlow
{
    public readonly struct BattleFlowSettings
    {
        public int RoundCount { get; }
        public float MatchPhaseDuration { get; }
        public float HeroPhaseDuration { get; }
        public int PrePhaseDuration { get; }
        public int CountdownThreshold { get; }
        public EnergyCarryoverMode EnergyCarryoverMode { get; }


        public BattleFlowSettings(
            int roundCount,
            float matchPhaseDuration,
            float heroPhaseDuration,
            int prePhaseDuration,
            int countdownThreshold,
            EnergyCarryoverMode energyCarryoverMode)
        {
            RoundCount = roundCount < 1 ? 1 : roundCount;
            MatchPhaseDuration = matchPhaseDuration < 0f ? 0f : matchPhaseDuration;
            HeroPhaseDuration = heroPhaseDuration < 0f ? 0f : heroPhaseDuration;
            PrePhaseDuration = prePhaseDuration < 0 ? 0 : prePhaseDuration;
            CountdownThreshold = countdownThreshold < 0 ? 0 : countdownThreshold;
            EnergyCarryoverMode = energyCarryoverMode;
        }
    }
}