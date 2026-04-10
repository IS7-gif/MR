using System;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Services.Combat;
using Project.Scripts.Services.Game;
using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Services.Timer
{
    public class OvertimeService : IOvertimeService
    {
        private readonly BattleTimerConfig _config;
        private readonly IHeroService _heroService;
        private readonly IPlayerStateService _playerState;
        private readonly IEnemyStateService _enemyState;
        private readonly IAvatarGroupDefenseService _groupDefense;
        private readonly IGameStateService _gameStateService;

        private bool _isActive;
        private float _drainAccumulator;


        public bool IsActive => _isActive;


        public OvertimeService(
            BattleTimerConfig config,
            IHeroService heroService,
            IPlayerStateService playerState,
            IEnemyStateService enemyState,
            IAvatarGroupDefenseService groupDefense,
            IGameStateService gameStateService)
        {
            _config = config;
            _heroService = heroService;
            _playerState = playerState;
            _enemyState = enemyState;
            _groupDefense = groupDefense;
            _gameStateService = gameStateService;
        }


        public void Begin()
        {
            _isActive = true;
            _drainAccumulator = 0f;
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

            DrainHeroes(BattleSide.Player, heroDamage);
            DrainHeroes(BattleSide.Enemy, heroDamage);

            if (_groupDefense.IsExposed(BattleSide.Player))
                _playerState.ForceApplyDamage(avatarDamage);

            if (_groupDefense.IsExposed(BattleSide.Enemy))
                _enemyState.ForceApplyDamage(avatarDamage);
        }

        private void DrainHeroes(BattleSide side, int damage)
        {
            var slots = _heroService.GetSlots(side);
            for (var i = 0; i < slots.Count; i++)
            {
                if (false == slots[i].IsAlive)
                    continue;

                _heroService.ApplyDamageToHero(side, i, damage, silent: true);
            }
        }
    }
}