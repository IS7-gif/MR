using System;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Rules;
using R3;

namespace Project.Scripts.Services.Combat
{
    public class PlayerStateService : IPlayerStateService, IDisposable
    {
        public int CurrentHP { get; private set; }
        public int MaxHP { get; }


        private readonly EventBus _eventBus;
        private readonly IAvatarGroupDefenseService _groupDefense;
        private readonly CompositeDisposable _subscriptions = new();


        public PlayerStateService(EventBus eventBus, LevelConfig levelConfig, IAvatarGroupDefenseService groupDefense)
        {
            _eventBus = eventBus;
            _groupDefense = groupDefense;
            MaxHP = levelConfig.PlayerAvatarConfig.MaxHP;
            CurrentHP = levelConfig.PlayerAvatarConfig.MaxHP;

            _subscriptions.Add(_eventBus.Subscribe<EnemyAvatarActivatedEvent>(OnEnemyAvatarActivated));
            _subscriptions.Add(_eventBus.Subscribe<HeroActivatedEvent>(OnHeroActivated));
        }


        public void Heal(int amount)
        {
            if (amount <= 0)
                return;

            ApplyHealthDelta(amount);
        }

        public void TakeDamage(int amount)
        {
            if (amount <= 0)
                return;

            if (false == _groupDefense.IsExposed(BattleSide.Player))
                return;

            ApplyHealthDelta(-amount);
        }

        public void ForceApplyDamage(int amount, bool suppressDefeatedEvent = false)
        {
            if (amount <= 0)
                return;

            ApplyHealthDelta(-amount, silent: true, suppressDefeatedEvent: suppressDefeatedEvent);
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }

        private void ApplyHealthDelta(int delta, bool silent = false, bool suppressDefeatedEvent = false)
        {
            var result = HealthChangeRules.Apply(CurrentHP, MaxHP, delta);
            if (false == result.WasChanged)
                return;

            CurrentHP = result.CurrentHP;
            _eventBus.Publish(new PlayerHPChangedEvent(CurrentHP, MaxHP, silent));

            if (result.BecameDefeated && false == suppressDefeatedEvent)
                _eventBus.Publish(new PlayerDefeatedEvent());
        }


        private void OnEnemyAvatarActivated(EnemyAvatarActivatedEvent e)
        {
            if (e.ActionType == HeroActionType.DealDamage)
                TakeDamage(e.ActionValue);
        }

        private void OnHeroActivated(HeroActivatedEvent e)
        {
            if (e.ActionType == HeroActionType.DealDamage && e.Side == BattleSide.Enemy)
                TakeDamage(e.ActionValue);
        }
    }
}