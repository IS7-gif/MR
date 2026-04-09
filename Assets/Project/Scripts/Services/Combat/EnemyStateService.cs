using System;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared.Heroes;
using R3;
using UnityEngine;

namespace Project.Scripts.Services.Combat
{
    public class EnemyStateService : IEnemyStateService, IDisposable
    {
        public int CurrentHP { get; private set; }
        public int MaxHP { get; }


        private readonly EventBus _eventBus;
        private readonly IAvatarGroupDefenseService _groupDefense;
        private readonly CompositeDisposable _subscriptions = new();


        public EnemyStateService(EventBus eventBus, LevelConfig levelConfig, IAvatarGroupDefenseService groupDefense)
        {
            _eventBus = eventBus;
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
            if (CurrentHP <= 0)
                return;

            if (!_groupDefense.IsExposed(BattleSide.Enemy))
                return;

            Debug.Log($"[Combat] Damage applied to enemy for {amount} (HP: {CurrentHP} → {Math.Max(0, CurrentHP - amount)}/{MaxHP})");
            CurrentHP = Math.Max(0, CurrentHP - amount);
            _eventBus.Publish(new EnemyHPChangedEvent(CurrentHP, MaxHP));

            if (CurrentHP == 0)
                _eventBus.Publish(new EnemyDefeatedEvent());
        }

        public void ApplyHeal(int amount)
        {
            if (CurrentHP >= MaxHP || amount <= 0)
                return;

            CurrentHP = Math.Min(MaxHP, CurrentHP + amount);
            _eventBus.Publish(new EnemyHPChangedEvent(CurrentHP, MaxHP));
        }


        private void OnEnemyAvatarActivated(EnemyAvatarActivatedEvent e)
        {
            if (e.ActionType == HeroActionType.HealAlly)
                ApplyHeal(e.ActionValue);
        }

        private void OnHeroActivated(HeroActivatedEvent e)
        {
            if (e.Side == BattleSide.Enemy && e.ActionType == HeroActionType.HealAlly)
                ApplyHeal(e.ActionValue);
        }
    }
}