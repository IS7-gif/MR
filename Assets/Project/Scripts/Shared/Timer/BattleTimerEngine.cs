namespace Project.Scripts.Shared.Timer
{
    public class BattleTimerEngine : IBattleTimerEngine
    {
        private float _timeRemaining;
        private bool _isOvertime;
        private bool _initialized;


        public BattleTimerSnapshot Snapshot => new(_timeRemaining, _isOvertime);


        public void Initialize(float battleDuration)
        {
            _timeRemaining = battleDuration;
            _isOvertime = false;
            _initialized = true;
        }

        public bool Tick(float deltaTime)
        {
            if (false == _initialized || _isOvertime)
                return false;

            _timeRemaining -= deltaTime;
            if (_timeRemaining <= 0f)
            {
                _timeRemaining = 0f;
                _isOvertime = true;
                return true;
            }

            return false;
        }
    }
}