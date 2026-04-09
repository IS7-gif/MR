using System;
using Project.Scripts.Shared.Heroes;
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
        public Observable<int> Hit => _hit;
        public Observable<int> Heal => _heal;


        private readonly Subject<int> _hit = new();
        private readonly Subject<int> _heal = new();
        private int _prevHP;


        public HeroSlotViewModel(
            int slotIndex,
            BattleSide side,
            HeroSlotState state,
            Color color,
            Sprite portrait)
        {
            SlotIndex = slotIndex;
            Side = side;
            IsAssigned = state.IsAssigned;
            ActionType = state.ActionType;
            SlotColor = color;
            Portrait = portrait;

            HPFill = new ReactiveProperty<float>(state.IsAssigned && state.MaxHP > 0 ? (float)state.CurrentHP / state.MaxHP : 1f);
            _prevHP = state.IsAssigned ? state.CurrentHP : 0;
        }

        public void UpdateEnergy(int current, int max)
        {
            EnergyFill.Value = max > 0 ? (float)current / max : 0f;
            IsActivatable.Value = IsAssigned && EnergyFill.Value >= 1f;
        }

        public void UpdateHP(int current, int max)
        {
            if (current < _prevHP)
                _hit.OnNext(_prevHP - current);
            else if (current > _prevHP)
                _heal.OnNext(current - _prevHP);

            _prevHP = current;
            HPFill.Value = max > 0 ? (float)current / max : 0f;

            if (current <= 0)
                IsDefeated.Value = true;
        }

        public void Dispose()
        {
            EnergyFill.Dispose();
            IsActivatable.Dispose();
            HPFill.Dispose();
            IsDefeated.Dispose();
            _hit.Dispose();
            _heal.Dispose();
        }
    }
}
