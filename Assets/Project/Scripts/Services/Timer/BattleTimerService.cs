using Project.Scripts.Configs.Battle;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Game;
using Project.Scripts.Shared.Timer;

namespace Project.Scripts.Services.Timer
{
    public class BattleTimerService : IBattleTimerService
    {
        private readonly BattleTimerConfig _config;
        private readonly EventBus _eventBus;
        private readonly IOvertimeTransitionCoordinator _overtimeTransitionCoordinator;
        private readonly IGameStateService _gameStateService;
        private readonly BattleTimerEngine _engine = new();

        private float _lastPublishedSecond;


        public bool IsRunning { get; private set; }


        public BattleTimerService(
            BattleTimerConfig config,
            EventBus eventBus,
            IOvertimeTransitionCoordinator overtimeTransitionCoordinator,
            IGameStateService gameStateService)
        {
            _config = config;
            _eventBus = eventBus;
            _overtimeTransitionCoordinator = overtimeTransitionCoordinator;
            _gameStateService = gameStateService;
        }


        public void Initialize()
        {
            _engine.Initialize(_config.BattleDuration);
            _lastPublishedSecond = _config.BattleDuration;
            IsRunning = true;

            PublishTimerChanged();
        }

        public void Tick(float deltaTime)
        {
            if (false == IsRunning || false == _gameStateService.IsPlaying)
                return;

            var timerElapsed = _engine.Tick(deltaTime);

            if (timerElapsed)
            {
                IsRunning = false;
                PublishTimerChanged();
                _overtimeTransitionCoordinator.RequestStart();
                return;
            }

            var snapshot = _engine.Snapshot;
            var currentSecond = (int)snapshot.TimeRemaining;

            if (currentSecond < (int)_lastPublishedSecond)
            {
                _lastPublishedSecond = currentSecond;
                PublishTimerChanged();
            }
        }


        private void PublishTimerChanged()
        {
            var timeRemaining = _engine.Snapshot.TimeRemaining;
            _eventBus.Publish(new BattleTimerChangedEvent(timeRemaining));
        }
    }
}