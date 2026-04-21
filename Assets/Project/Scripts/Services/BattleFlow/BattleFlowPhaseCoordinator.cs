using System;
using Project.Scripts.Services.Combat;
using Project.Scripts.Services.Board;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Game;
using Project.Scripts.Services.Timer;
using Project.Scripts.Shared.BattleFlow;
using VContainer.Unity;
using R3;

namespace Project.Scripts.Services.BattleFlow
{
    public class BattleFlowPhaseCoordinator : IStartable, IDisposable
    {
        private readonly EventBus _eventBus;
        private readonly IBattleFlowService _battleFlowService;
        private readonly IBoardRuntimeService _boardRuntimeService;
        private readonly IBattleActionRuntimeService _battleActionRuntimeService;
        private readonly IGameStateService _gameStateService;
        private readonly IOvertimeTransitionCoordinator _overtimeTransitionCoordinator;
        private readonly IPlayerStateService _playerStateService;
        private readonly IEnemyStateService _enemyStateService;
        private IDisposable _phaseChangedSubscription;
        private IDisposable _gameStateSubscription;


        public BattleFlowPhaseCoordinator(
            EventBus eventBus,
            IBattleFlowService battleFlowService,
            IBoardRuntimeService boardRuntimeService,
            IBattleActionRuntimeService battleActionRuntimeService,
            IGameStateService gameStateService,
            IOvertimeTransitionCoordinator overtimeTransitionCoordinator,
            IPlayerStateService playerStateService,
            IEnemyStateService enemyStateService)
        {
            _eventBus = eventBus;
            _battleFlowService = battleFlowService;
            _boardRuntimeService = boardRuntimeService;
            _battleActionRuntimeService = battleActionRuntimeService;
            _gameStateService = gameStateService;
            _overtimeTransitionCoordinator = overtimeTransitionCoordinator;
            _playerStateService = playerStateService;
            _enemyStateService = enemyStateService;
        }


        public void Start()
        {
            _phaseChangedSubscription = _eventBus.Subscribe<BattleFlowPhaseChangedEvent>(OnBattleFlowPhaseChanged);
            _gameStateSubscription = _gameStateService.State.Subscribe(OnGameStateChanged);

            if (_battleFlowService.IsInitialized)
                ApplyPhase(_battleFlowService.Snapshot.Phase);
        }

        public void Dispose()
        {
            _phaseChangedSubscription?.Dispose();
            _phaseChangedSubscription = null;
            _gameStateSubscription?.Dispose();
            _gameStateSubscription = null;
        }


        private void OnBattleFlowPhaseChanged(BattleFlowPhaseChangedEvent e)
        {
            ApplyPhase(e.Phase);
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state != GameState.Win && state != GameState.Lose)
                return;

            _battleFlowService.MarkFinished();
        }

        private void ApplyPhase(BattlePhaseKind phase)
        {
            _boardRuntimeService.ApplyBattleFlowPhase(phase);
            _battleActionRuntimeService.ApplyBattleFlowPhase(phase);

            if (phase == BattlePhaseKind.PendingOvertime && _playerStateService.CurrentHP > 0
                                                         && _enemyStateService.CurrentHP > 0)
            {
                _overtimeTransitionCoordinator.RequestStart();
            }
        }
    }
}