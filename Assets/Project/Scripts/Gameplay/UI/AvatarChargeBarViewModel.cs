using System;
using Project.Scripts.Services.EventBusSystem;
using Project.Scripts.Services.EventBusSystem.Events;
using Project.Scripts.Shared.Heroes;
using R3;

namespace Project.Scripts.Gameplay.UI
{
    public class AvatarChargeBarViewModel : IDisposable
    {
        public ReactiveProperty<float> FillFraction { get; } = new(0f);
        public ReactiveProperty<bool> IsFull { get; } = new(false);

        
        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();


        public AvatarChargeBarViewModel(EventBus eventBus, BattleSide side)
        {
            if (side == BattleSide.Player)
                _subscriptions.Add(eventBus.Subscribe<PlayerChargeChangedEvent>(OnPlayerChargeChanged));
            else
                _subscriptions.Add(eventBus.Subscribe<EnemyChargeChangedEvent>(OnEnemyChargeChanged));
        }

        public void Dispose()
        {
            FillFraction.Dispose();
            IsFull.Dispose();
            _subscriptions.Dispose();
        }


        private void OnPlayerChargeChanged(PlayerChargeChangedEvent e)
        {
            ApplyCharge(e.Current, e.Max);
        }

        private void OnEnemyChargeChanged(EnemyChargeChangedEvent e)
        {
            ApplyCharge(e.Current, e.Max);
        }

        private void ApplyCharge(int current, int max)
        {
            FillFraction.Value = max > 0 ? (float)current / max : 0f;
            IsFull.Value = max > 0 && current >= max;
        }
    }
}