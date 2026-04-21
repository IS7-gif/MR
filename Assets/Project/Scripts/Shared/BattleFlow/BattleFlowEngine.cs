namespace Project.Scripts.Shared.BattleFlow
{
    public class BattleFlowEngine
    {
        public BattleFlowSnapshot Snapshot => _snapshot;


        private BattleFlowSettings _settings;
        private BattleFlowSnapshot _snapshot;
        private bool _initialized;


        public void Initialize(BattleFlowSettings settings)
        {
            _settings = settings;
            _snapshot = new BattleFlowSnapshot(
                currentRound: 1,
                totalRounds: settings.RoundCount,
                phase: BattlePhaseKind.Match,
                timeRemaining: settings.MatchPhaseDuration,
                energyCarryoverMode: settings.EnergyCarryoverMode);
            _initialized = true;
        }

        public bool Tick(float deltaTime)
        {
            if (false == _initialized || deltaTime <= 0f || _snapshot.IsTerminal)
                return false;

            var nextTimeRemaining = _snapshot.TimeRemaining - deltaTime;
            if (nextTimeRemaining > 0f)
            {
                _snapshot = _snapshot.WithTimeRemaining(nextTimeRemaining);
                return false;
            }

            AdvancePhase();
            return true;
        }

        public void MarkFinished()
        {
            if (false == _initialized)
                return;

            _snapshot = new BattleFlowSnapshot(
                _snapshot.CurrentRound,
                _snapshot.TotalRounds,
                BattlePhaseKind.Finished,
                0f,
                _snapshot.EnergyCarryoverMode);
        }


        private void AdvancePhase()
        {
            if (_snapshot.Phase == BattlePhaseKind.Match)
            {
                _snapshot = new BattleFlowSnapshot(
                    _snapshot.CurrentRound,
                    _snapshot.TotalRounds,
                    BattlePhaseKind.Hero,
                    _settings.HeroPhaseDuration,
                    _snapshot.EnergyCarryoverMode);
                return;
            }

            if (_snapshot.Phase == BattlePhaseKind.Hero && _snapshot.CurrentRound < _snapshot.TotalRounds)
            {
                _snapshot = new BattleFlowSnapshot(
                    _snapshot.CurrentRound + 1,
                    _snapshot.TotalRounds,
                    BattlePhaseKind.Match,
                    _settings.MatchPhaseDuration,
                    _snapshot.EnergyCarryoverMode);
                return;
            }

            _snapshot = new BattleFlowSnapshot(
                _snapshot.CurrentRound,
                _snapshot.TotalRounds,
                BattlePhaseKind.PendingOvertime,
                0f,
                _snapshot.EnergyCarryoverMode);
        }
    }
}