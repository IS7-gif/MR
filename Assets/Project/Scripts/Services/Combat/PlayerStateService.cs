using System;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared.Heroes;
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
            if (CurrentHP >= MaxHP || amount <= 0)
                return;

            CurrentHP = Math.Min(MaxHP, CurrentHP + amount);
            _eventBus.Publish(new PlayerHPChangedEvent(CurrentHP, MaxHP));
        }

        public void TakeDamage(int amount)
        {
            if (CurrentHP <= 0 || amount <= 0)
                return;

            if (!_groupDefense.IsExposed(BattleSide.Player))
                return;

            CurrentHP = Math.Max(0, CurrentHP - amount);
            _eventBus.Publish(new PlayerHPChangedEvent(CurrentHP, MaxHP));

            if (CurrentHP == 0)
                _eventBus.Publish(new PlayerDefeatedEvent());
        }

        public void ForceApplyDamage(int amount)
        {
            if (CurrentHP <= 0 || amount <= 0)
                return;

            CurrentHP = Math.Max(0, CurrentHP - amount);
            _eventBus.Publish(new PlayerHPChangedEvent(CurrentHP, MaxHP, silent: true));

            if (CurrentHP == 0)
                _eventBus.Publish(new PlayerDefeatedEvent());
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
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