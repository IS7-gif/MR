using System;
using Project.Scripts.Shared.Heroes;
using R3;
using UnityEngine;

namespace Project.Scripts.Gameplay.UI
{
    public class HeroSlotViewModel : IDisposable
    {
        public bool IsAssigned { get; }
        public Color SlotColor { get; }
        public Sprite Portrait { get; }
        public int SlotIndex { get; }
        public BattleSide Side { get; }
        public bool IsPlayerSlot => Side == BattleSide.Player;

        public ReactiveProperty<float> EnergyFill { get; } = new(0f);
        public ReactiveProperty<bool> IsActivatable { get; } = new(false);

        public Action<int> OnActivateClicked { get; }


        public HeroSlotViewModel(
            int slotIndex,
            BattleSide side,
            HeroSlotState state,
            Color color,
            Sprite portrait,
            Action<int> onActivate)
        {
            SlotIndex = slotIndex;
            Side = side;
            IsAssigned = state.IsAssigned;
            SlotColor = color;
            Portrait = portrait;
            OnActivateClicked = onActivate;
        }

        public void UpdateEnergy(int current, int max)
        {
            EnergyFill.Value = max > 0 ? (float)current / max : 0f;
            IsActivatable.Value = IsAssigned && EnergyFill.Value >= 1f;
        }

        public void Dispose()
        {
            EnergyFill.Dispose();
            IsActivatable.Dispose();
        }
    }
}