using System;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared.Heroes;
using R3;

namespace Project.Scripts.Gameplay.Battle.Units
{
    public class AvatarChargeBarViewModel : IDisposable
    {
        public ReactiveProperty<float> FillFraction { get; } = new(0f);
        public ReactiveProperty<bool> IsReady { get; } = new(false);


        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();


        public AvatarChargeBarViewModel(EventBus eventBus, BattleSide side)
        {
            if (side == BattleSide.Player)
                _subscriptions.Add(eventBus.Subscribe<PlayerAvatarEnergyChangedEvent>(OnPlayerEnergyChanged));
            else
                _subscriptions.Add(eventBus.Subscribe<EnemyAvatarEnergyChangedEvent>(OnEnemyEnergyChanged));
        }

        public void Dispose()
        {
            FillFraction.Dispose();
            IsReady.Dispose();
            _subscriptions.Dispose();
        }


        private void OnPlayerEnergyChanged(PlayerAvatarEnergyChangedEvent e)
        {
            ApplyEnergy(e.Current, e.Max);
        }

        private void OnEnemyEnergyChanged(EnemyAvatarEnergyChangedEvent e)
        {
            ApplyEnergy(e.Current, e.Max);
        }

        private void ApplyEnergy(int current, int max)
        {
            FillFraction.Value = max > 0 ? (float)current / max : 0f;
            IsReady.Value = max > 0 && current >= max;
        }
    }
}
