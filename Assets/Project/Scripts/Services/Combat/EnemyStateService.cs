using System;
using Project.Scripts.Configs;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Rules;
using R3;
using UnityEngine;

namespace Project.Scripts.Services.Combat
{
    public class EnemyStateService : IEnemyStateService, IDisposable
    {
        public int CurrentHP { get; private set; }
        public int MaxHP { get; }


        private readonly EventBus _eventBus;
        private readonly DebugConfig _debugConfig;
        private readonly IAvatarGroupDefenseService _groupDefense;
        private readonly CompositeDisposable _subscriptions = new();


        public EnemyStateService(EventBus eventBus, DebugConfig debugConfig, LevelConfig levelConfig, IAvatarGroupDefenseService groupDefense)
        {
            _eventBus = eventBus;
            _debugConfig = debugConfig;
            _groupDefense = groupDefense;
            MaxHP = levelConfig.EnemyAvatarConfig.MaxHP;
            CurrentHP = levelConfig.EnemyAvatarConfig.MaxHP;

            _subscriptions.Add(_eventBus.Subscribe<EnemyAvatarActivatedEvent>(OnEnemyAvatarActivated));
            _subscriptions.Add(_eventBus.Subscribe<HeroActivatedEvent>(OnHeroActivated));
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }


        public void ApplyDamage(int amount)
        {
            if (amount <= 0)
                return;

            if (false == _groupDefense.IsExposed(BattleSide.Enemy))
                return;

            if (_debugConfig.LogCombatDamage)
                Debug.Log($"[Combat] Damage applied to enemy for {amount} (HP: {CurrentHP} → {Math.Max(0, CurrentHP - amount)}/{MaxHP})");
            ApplyHealthDelta(-amount);
        }

        public void ApplyHeal(int amount)
        {
            if (amount <= 0)
                return;

            ApplyHealthDelta(amount);
        }

        public void ForceApplyDamage(int amount)
        {
            if (amount <= 0)
                return;

            ApplyHealthDelta(-amount, silent: true);
        }

        private void ApplyHealthDelta(int delta, bool silent = false)
        {
            var result = HealthChangeRules.Apply(CurrentHP, MaxHP, delta);
            if (false == result.WasChanged)
                return;

            CurrentHP = result.CurrentHP;
            _eventBus.Publish(new EnemyHPChangedEvent(CurrentHP, MaxHP, silent));

            if (result.BecameDefeated)
                _eventBus.Publish(new EnemyDefeatedEvent());
        }


        private void OnEnemyAvatarActivated(EnemyAvatarActivatedEvent e)
        {
            if (e.ActionType == HeroActionType.HealAlly)
                ApplyHeal(e.ActionValue);
        }

        private void OnHeroActivated(HeroActivatedEvent e)
        {
            if (e is { Side: BattleSide.Enemy, ActionType: HeroActionType.HealAlly })
                ApplyHeal(e.ActionValue);
        }
    }
}