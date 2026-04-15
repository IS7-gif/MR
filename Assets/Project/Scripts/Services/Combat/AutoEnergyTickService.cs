using System;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared.Timer;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Project.Scripts.Services.Combat
{
    public class AutoEnergyTickService : IStartable, ITickable, IDisposable, IBattleEscalationModifier
    {
        private readonly EventBus _eventBus;
        private readonly AutoEnergyConfig _autoEnergyConfig;
        private readonly EscalationConfig _escalationConfig;
        private readonly CompositeDisposable _subscriptions = new();

        private float _elapsed;
        private bool _isActive = true;
        private float _intervalMultiplier = 1f;
        private float _amountMultiplier = 1f;


        public AutoEnergyTickService(EventBus eventBus, AutoEnergyConfig autoEnergyConfig, EscalationConfig escalationConfig)
        {
            _eventBus = eventBus;
            _autoEnergyConfig = autoEnergyConfig;
            _escalationConfig = escalationConfig;
        }

        public void Start()
        {
            _subscriptions.Add(_eventBus.Subscribe<OvertimeStartedEvent>(_ => _isActive = false));
        }

        public void Tick()
        {
            if (!_isActive)
                return;

            _elapsed += Time.deltaTime;

            var interval = _autoEnergyConfig.TickInterval / _intervalMultiplier;
            if (_elapsed < interval)
                return;

            _elapsed = 0f;

            var amount = Mathf.Max(0.01f, _autoEnergyConfig.EnergyPerTick * _amountMultiplier);
            _eventBus.Publish(new AutoEnergyTickEvent(amount));
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }


        public void OnEscalationReached()
        {
            SetIntervalMultiplier(_escalationConfig.AutoEnergyIntervalMultiplier);
        }

        public void SetIntervalMultiplier(float multiplier)
        {
            _intervalMultiplier = Mathf.Max(0.01f, multiplier);
        }

        public void SetAmountMultiplier(float multiplier)
        {
            _amountMultiplier = Mathf.Max(0f, multiplier);
        }
    }
}