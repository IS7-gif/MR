using System;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Game;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Project.Scripts.Services.Combat
{
    public class AutoEnergyTickService : IStartable, ITickable, IDisposable
    {
        private readonly EventBus _eventBus;
        private readonly AutoEnergyConfig _autoEnergyConfig;
        private readonly IBattleEconomyModifierService _battleEconomyModifier;
        private readonly IGameStateService _gameStateService;
        private readonly CompositeDisposable _subscriptions = new();

        private float _elapsed;
        private bool _isActive = true;


        public AutoEnergyTickService(
            EventBus eventBus,
            AutoEnergyConfig autoEnergyConfig,
            IBattleEconomyModifierService battleEconomyModifier,
            IGameStateService gameStateService)
        {
            _eventBus = eventBus;
            _autoEnergyConfig = autoEnergyConfig;
            _battleEconomyModifier = battleEconomyModifier;
            _gameStateService = gameStateService;
        }

        public void Start()
        {
            _subscriptions.Add(_eventBus.Subscribe<BurndownStartedEvent>(_ => _isActive = false));
            _subscriptions.Add(_gameStateService.State.Subscribe(OnGameStateChanged));
        }

        public void Tick()
        {
            if (!_isActive)
                return;

            if (false == _gameStateService.IsPlaying)
                return;

            _elapsed += Time.deltaTime;

            var interval = _autoEnergyConfig.TickInterval / _battleEconomyModifier.AutoEnergyIntervalMultiplier;
            if (_elapsed < interval)
                return;

            _elapsed = 0f;

            var amount = Mathf.Max(0.01f, _autoEnergyConfig.EnergyPerTick);
            _eventBus.Publish(new AutoEnergyTickEvent(amount));
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state != GameState.Playing)
                _isActive = false;
        }
    }
}