using System;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Services.Combat;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Game;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Timer;
using Random = System.Random;

namespace Project.Scripts.Services.Timer
{
    public class BurndownService : IBurndownService
    {
        private readonly BurndownConfig _config;
        private readonly IHeroService _heroService;
        private readonly IPlayerStateService _playerState;
        private readonly IEnemyStateService _enemyState;
        private readonly EventBus _eventBus;
        private readonly IGameStateService _gameStateService;

        private readonly Random _random = new();

        private bool _isActive;
        private float _drainAccumulator;
        private BurndownDrainCursor _playerCursor;
        private BurndownDrainCursor _enemyCursor;


        public bool IsActive => _isActive;


        public BurndownService(
            BurndownConfig config,
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
            if (_isActive)
                return;

            _isActive = true;
            _drainAccumulator = 0f;

            _playerCursor.Initialize(_heroService.GetSlots(BattleSide.Player));
            _enemyCursor.Initialize(_heroService.GetSlots(BattleSide.Enemy));

            _eventBus.Publish(new BurndownDrainTargetChangedEvent(BattleSide.Player, _playerCursor.TargetIndex));
            _eventBus.Publish(new BurndownDrainTargetChangedEvent(BattleSide.Enemy, _enemyCursor.TargetIndex));
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

            var playerHpBefore = _playerState.CurrentHP;
            var enemyHpBefore = _enemyState.CurrentHP;

            DrainSide(BattleSide.Player, ref _playerCursor, heroDamage, avatarDamage);
            DrainSide(BattleSide.Enemy, ref _enemyCursor, heroDamage, avatarDamage);

            var playerDied = playerHpBefore > 0 && _playerState.CurrentHP <= 0;
            var enemyDied = enemyHpBefore > 0 && _enemyState.CurrentHP <= 0;

            if (playerDied && enemyDied)
                ResolveSimultaneousDeath(playerHpBefore, enemyHpBefore);
            else if (playerDied)
                _eventBus.Publish(new PlayerDefeatedEvent());
            else if (enemyDied)
                _eventBus.Publish(new EnemyDefeatedEvent());
        }


        private void ResolveSimultaneousDeath(int playerHpBefore, int enemyHpBefore)
        {
            bool playerWins;
            if (playerHpBefore != enemyHpBefore)
                playerWins = playerHpBefore > enemyHpBefore;
            else
                playerWins = _random.Next(2) == 0; // TODO: replace with draw system

            if (playerWins)
                _eventBus.Publish(new EnemyDefeatedEvent());
            else
                _eventBus.Publish(new PlayerDefeatedEvent());
        }

        private void DrainSide(BattleSide side, ref BurndownDrainCursor cursor, int heroDamage, int avatarDamage)
        {
            var slots = _heroService.GetSlots(side);

            if (cursor.AdvanceIfDead(slots))
                _eventBus.Publish(new BurndownDrainTargetChangedEvent(side, cursor.TargetIndex));

            if (cursor.IsDrainingAvatar)
            {
                if (side == BattleSide.Player)
                    _playerState.ForceApplyDamage(avatarDamage, suppressDefeatedEvent: true);
                else
                    _enemyState.ForceApplyDamage(avatarDamage, suppressDefeatedEvent: true);
                return;
            }

            var idx = cursor.TargetIndex;
            _heroService.ApplyDamageToHero(side, idx, heroDamage, silent: true);

            if (false == slots[idx].IsAlive)
            {
                cursor.AdvanceIfDead(slots);
                _eventBus.Publish(new BurndownDrainTargetChangedEvent(side, cursor.TargetIndex));
            }
        }
    }
}