using System;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Services.Combat;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Game;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Timer;

namespace Project.Scripts.Services.Timer
{
    public class OvertimeService : IOvertimeService
    {
        private readonly BattleTimerConfig _config;
        private readonly IHeroService _heroService;
        private readonly IPlayerStateService _playerState;
        private readonly IEnemyStateService _enemyState;
        private readonly EventBus _eventBus;
        private readonly IGameStateService _gameStateService;

        private bool _isActive;
        private float _drainAccumulator;
        private OvertimeDrainCursor _playerCursor;
        private OvertimeDrainCursor _enemyCursor;


        public bool IsActive => _isActive;


        public OvertimeService(
            BattleTimerConfig config,
            IHeroService heroService,
            IPlayerStateService playerState,
            IEnemyStateService enemyState,
            EventBus eventBus,
            IGameStateService gameStateService)
        {
            _config = config;
            _heroService = heroService;
            _playerState = playerState;
            _enemyState = enemyState;
            _eventBus = eventBus;
            _gameStateService = gameStateService;
        }


        public void Begin()
        {
            _isActive = true;
            _drainAccumulator = 0f;

            _playerCursor.Initialize(_heroService.GetSlots(BattleSide.Player));
            _enemyCursor.Initialize(_heroService.GetSlots(BattleSide.Enemy));

            _eventBus.Publish(new OvertimeDrainTargetChangedEvent(BattleSide.Player, _playerCursor.TargetIndex));
            _eventBus.Publish(new OvertimeDrainTargetChangedEvent(BattleSide.Enemy, _enemyCursor.TargetIndex));
        }

        public void Tick(float deltaTime)
        {
            if (false == _isActive)
                return;

            var state = _gameStateService.State.CurrentValue;
            if (state == GameState.Win || state == GameState.Lose)
                return;

            _drainAccumulator += deltaTime;

            if (_drainAccumulator < _config.DrainTickInterval)
                return;

            var ticks = (int)(_drainAccumulator / _config.DrainTickInterval);
            _drainAccumulator -= ticks * _config.DrainTickInterval;

            var heroDamage = (int)Math.Ceiling(_config.HeroDrainPerSecond * _config.DrainTickInterval * ticks);
            var avatarDamage = (int)Math.Ceiling(_config.AvatarDrainPerSecond * _config.DrainTickInterval * ticks);

            DrainSide(BattleSide.Player, ref _playerCursor, heroDamage, avatarDamage);
            DrainSide(BattleSide.Enemy, ref _enemyCursor, heroDamage, avatarDamage);
        }


        private void DrainSide(BattleSide side, ref OvertimeDrainCursor cursor, int heroDamage, int avatarDamage)
        {
            var slots = _heroService.GetSlots(side);

            if (cursor.AdvanceIfDead(slots))
                _eventBus.Publish(new OvertimeDrainTargetChangedEvent(side, cursor.TargetIndex));

            if (cursor.IsDrainingAvatar)
            {
                if (side == BattleSide.Player)
                    _playerState.ForceApplyDamage(avatarDamage);
                else
                    _enemyState.ForceApplyDamage(avatarDamage);
                return;
            }

            var idx = cursor.TargetIndex;
            _heroService.ApplyDamageToHero(side, idx, heroDamage, silent: true);

            if (false == slots[idx].IsAlive)
            {
                cursor.AdvanceIfDead(slots);
                _eventBus.Publish(new OvertimeDrainTargetChangedEvent(side, cursor.TargetIndex));
            }
        }
    }
}