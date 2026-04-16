using System;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Game;
using Project.Scripts.Shared.Heroes;
using R3;
using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.Units
{
    public class AvatarSlotViewModel : IDisposable
    {
        public BattleSide Side { get; }
        public Color SlotColor { get; }
        public Sprite Portrait { get; }
        public BattleAnimationConfig AnimConfig { get; }
        public EventBus EventBus { get; }
        public HeroActionType AbilityType { get; }
        public ReactiveProperty<float> HPFill { get; }
        public ReactiveProperty<bool> IsDefeated { get; } = new(false);
        public Observable<HealthBarUpdate> HealthBarUpdated => _healthBarUpdated;
        public Observable<int> Hit => _hit;
        public Observable<int> Heal => _heal;
        public AvatarChargeBarViewModel EnergyBar { get; }


        private readonly Subject<HealthBarUpdate> _healthBarUpdated = new();
        private readonly Subject<int> _hit = new();
        private readonly Subject<int> _heal = new();
        private readonly CompositeDisposable _subscriptions = new();
        private int _prevHP;


        public AvatarSlotViewModel(EventBus eventBus, BattleSide side, Color slotColor, Sprite portrait,
            int initialHP, int maxHP, BattleAnimationConfig animConfig, HeroActionType abilityType,
            IBattleActionRuntimeService battleActionRuntimeService)
        {
            Side = side;
            SlotColor = slotColor;
            Portrait = portrait;
            AnimConfig = animConfig;
            EventBus = eventBus;
            AbilityType = abilityType;
            _prevHP = initialHP;
            HPFill = new ReactiveProperty<float>(maxHP > 0 ? (float)initialHP / maxHP : 1f);
            IsDefeated.Value = initialHP <= 0;
            EnergyBar = new AvatarChargeBarViewModel(eventBus, side, battleActionRuntimeService);

            if (side == BattleSide.Player)
            {
                _subscriptions.Add(eventBus.Subscribe<PlayerHPChangedEvent>(OnPlayerHPChanged));
                _subscriptions.Add(eventBus.Subscribe<PlayerDefeatedEvent>(_ => OnDefeated()));
            }
            else
            {
                _subscriptions.Add(eventBus.Subscribe<EnemyHPChangedEvent>(OnEnemyHPChanged));
                _subscriptions.Add(eventBus.Subscribe<EnemyDefeatedEvent>(_ => OnDefeated()));
            }
        }

        public void Dispose()
        {
            HPFill.Dispose();
            IsDefeated.Dispose();
            _healthBarUpdated.Dispose();
            _hit.Dispose();
            _heal.Dispose();
            EnergyBar.Dispose();
            _subscriptions.Dispose();
        }


        private void OnPlayerHPChanged(PlayerHPChangedEvent e)
        {
            ApplyHPChanged(e.Current, e.Max, e.Silent);
        }

        private void OnEnemyHPChanged(EnemyHPChangedEvent e)
        {
            ApplyHPChanged(e.Current, e.Max, e.Silent);
        }

        private void ApplyHPChanged(int current, int max, bool silent = false)
        {
            var fill = max > 0 ? (float)current / max : 0f;
            var previousHP = _prevHP;

            _prevHP = current;
            HPFill.Value = fill;

            if (silent)
            {
                _healthBarUpdated.OnNext(new HealthBarUpdate(fill, HealthBarUpdateMode.Snap));

                if (current <= 0)
                    IsDefeated.Value = true;

                return;
            }

            if (current < previousHP)
            {
                _hit.OnNext(previousHP - current);
                _healthBarUpdated.OnNext(new HealthBarUpdate(fill, HealthBarUpdateMode.Damage));
            }
            else if (current > previousHP)
            {
                _heal.OnNext(current - previousHP);
                _healthBarUpdated.OnNext(new HealthBarUpdate(fill, HealthBarUpdateMode.Heal));
            }
            else
            {
                _healthBarUpdated.OnNext(new HealthBarUpdate(fill, HealthBarUpdateMode.Snap));
            }

            if (current <= 0)
                IsDefeated.Value = true;
        }

        private void OnDefeated()
        {
            IsDefeated.Value = true;
        }
    }
}