using Project.Scripts.Services.Board;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Game;

namespace Project.Scripts.Services.Timer
{
    public class OvertimeTransitionCoordinator : IOvertimeTransitionCoordinator
    {
        public bool IsStarted { get; private set; }


        private readonly EventBus _eventBus;
        private readonly IBoardRuntimeService _boardRuntimeService;
        private readonly IGameStateService _gameStateService;
        private readonly IOvertimeService _overtimeService;


        public OvertimeTransitionCoordinator(
            EventBus eventBus,
            IBoardRuntimeService boardRuntimeService,
            IGameStateService gameStateService,
            IOvertimeService overtimeService)
        {
            _eventBus = eventBus;
            _boardRuntimeService = boardRuntimeService;
            _gameStateService = gameStateService;
            _overtimeService = overtimeService;
        }


        public void RequestStart()
        {
            if (IsStarted)
                return;

            var state = _gameStateService.State.CurrentValue;
            if (state == GameState.Win || state == GameState.Lose)
                return;

            IsStarted = true;
            _boardRuntimeService.RequestOvertimeStop();

            if (state != GameState.Overtime)
                _gameStateService.SetState(GameState.Overtime);

            _eventBus.Publish(new OvertimeStartedEvent());
            _overtimeService.Begin();
        }
    }
}