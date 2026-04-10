using System;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared.Heroes;
using R3;
using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.Units
{
    public class AvatarSlotViewModel : IDisposable
    {
        public BattleSide Side { get; }
        public Sprite Portrait { get; }
        public BattleAnimationConfig AnimConfig { get; }
        public EventBus EventBus { get; }
        public HeroActionType AbilityType { get; }
        public ReactiveProperty<float> HPFill { get; }
        public Observable<int> Hit => _hit;
        public Observable<int> Heal => _heal;
        public Observable<float> SilentDrain => _silentDrain;
        public AvatarChargeBarViewModel EnergyBar { get; }


        private readonly Subject<int> _hit = new();
        private readonly Subject<int> _heal = new();
        private readonly Subject<float> _silentDrain = new();
        private readonly CompositeDisposable _subscriptions = new();
        private int _prevHP;


        public AvatarSlotViewModel(EventBus eventBus, BattleSide side, Sprite portrait,
            int initialHP, int maxHP, BattleAnimationConfig animConfig, HeroActionType abilityType)
        {
            Side = side;
            Portrait = portrait;
            AnimConfig = animConfig;
            EventBus = eventBus;
            AbilityType = abilityType;
            _prevHP = initialHP;
            HPFill = new ReactiveProperty<float>(maxHP > 0 ? (float)initialHP / maxHP : 1f);
            EnergyBar = new AvatarChargeBarViewModel(eventBus, side);

            if (side == BattleSide.Player)
                _subscriptions.Add(eventBus.Subscribe<PlayerHPChangedEvent>(OnPlayerHPChanged));
            else
                _subscriptions.Add(eventBus.Subscribe<EnemyHPChangedEvent>(OnEnemyHPChanged));
        }

        public void Dispose()
        {
            HPFill.Dispose();
            _hit.Dispose();
            _heal.Dispose();
            _silentDrain.Dispose();
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

            if (silent)
            {
                _prevHP = current;
                _silentDrain.OnNext(fill);
                return;
            }

            if (current < _prevHP)
                _hit.OnNext(_prevHP - current);
            else if (current > _prevHP)
                _heal.OnNext(current - _prevHP);

            _prevHP = current;
            HPFill.Value = fill;
        }
    }
}
