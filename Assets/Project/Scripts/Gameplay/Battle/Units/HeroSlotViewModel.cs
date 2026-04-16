using System;
using Project.Scripts.Services.Game;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Rules;
using R3;
using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.Units
{
    public class HeroSlotViewModel : IDisposable
    {
        public bool IsAssigned { get; }
        public Color SlotColor { get; }
        public Sprite Portrait { get; }
        public int SlotIndex { get; }
        public BattleSide Side { get; }
        public HeroActionType ActionType { get; }
        public bool IsPlayerSlot => Side == BattleSide.Player;
        public ReactiveProperty<float> EnergyFill { get; } = new(0f);
        public ReactiveProperty<bool>  IsActivatable { get; } = new(false);
        public ReactiveProperty<float> HPFill { get; }
        public ReactiveProperty<bool>  IsDefeated { get; } = new(false);
        public Observable<HealthBarUpdate> HealthBarUpdated => _healthBarUpdated;
        public Observable<int> Hit => _hit;
        public Observable<int> Heal => _heal;


        private readonly Subject<HealthBarUpdate> _healthBarUpdated = new();
        private readonly Subject<int> _hit = new();
        private readonly Subject<int> _heal = new();
        private readonly CompositeDisposable _subscriptions = new();
        private readonly IBattleActionRuntimeService _battleActionRuntimeService;
        private int _prevHP;
        private bool _isEnergyReady;


        public HeroSlotViewModel(
            int slotIndex,
            BattleSide side,
            HeroSlotState state,
            Color color,
            Sprite portrait,
            IBattleActionRuntimeService battleActionRuntimeService)
        {
            SlotIndex = slotIndex;
            Side = side;
            IsAssigned = state.IsAssigned;
            ActionType = state.ActionType;
            SlotColor = color;
            Portrait = portrait;
            _battleActionRuntimeService = battleActionRuntimeService;

            HPFill = new ReactiveProperty<float>(state.IsAssigned && state.MaxHP > 0 ? (float)state.CurrentHP / state.MaxHP : 1f);
            _prevHP = state.IsAssigned ? state.CurrentHP : 0;
            IsDefeated.Value = state.IsAssigned && state.CurrentHP <= 0;
            _isEnergyReady = state.IsAssigned && state.MaxEnergy > 0 && state.CurrentEnergy >= state.MaxEnergy;
            RefreshActivatable();

            _subscriptions.Add(_battleActionRuntimeService.State.Subscribe(_ => RefreshActivatable()));
        }

        public void UpdateEnergy(int current, int max)
        {
            EnergyFill.Value = max > 0 ? (float)current / max : 0f;
            _isEnergyReady = max > 0 && current >= max;
            RefreshActivatable();
        }

        public void UpdateHP(int current, int max, bool silent = false)
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

        public void Dispose()
        {
            EnergyFill.Dispose();
            IsActivatable.Dispose();
            HPFill.Dispose();
            IsDefeated.Dispose();
            _healthBarUpdated.Dispose();
            _hit.Dispose();
            _heal.Dispose();
            _subscriptions.Dispose();
        }


        private void RefreshActivatable()
        {
            var canActivate = _battleActionRuntimeService.Evaluate(BattleActionKind.HeroActivation).IsAllowed;
            IsActivatable.Value = IsAssigned && _isEnergyReady && canActivate;
        }
    }
}