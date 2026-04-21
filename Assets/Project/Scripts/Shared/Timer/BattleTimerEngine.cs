namespace Project.Scripts.Shared.Timer
{
    public class BattleTimerEngine : IBattleTimerEngine
    {
        private float _timeRemaining;
        private bool _hasElapsed;
        private bool _initialized;


        public BattleTimerSnapshot Snapshot => new(_timeRemaining);


        public void Initialize(float battleDuration)
        {
            _timeRemaining = battleDuration;
            _hasElapsed = false;
            _initialized = true;
        }

        public bool Tick(float deltaTime)
        {
            if (false == _initialized || _hasElapsed)
                return false;

            _timeRemaining -= deltaTime;
            if (_timeRemaining <= 0f)
            {
                _timeRemaining = 0f;
                _hasElapsed = true;
                return true;
            }

            return false;
        }
    }
}