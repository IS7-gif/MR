using System;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Game;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Rules;
using R3;

namespace Project.Scripts.Gameplay.Battle.Units
{
    public class AvatarChargeBarViewModel : IDisposable
    {
        public ReactiveProperty<float> FillFraction { get; } = new(0f);
        public ReactiveProperty<bool> IsReady { get; } = new(false);


        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
        private readonly IBattleActionRuntimeService _battleActionRuntimeService;
        private bool _isEnergyReady;


        public AvatarChargeBarViewModel(EventBus eventBus, BattleSide side, IBattleActionRuntimeService battleActionRuntimeService)
        {
            _battleActionRuntimeService = battleActionRuntimeService;

            if (side == BattleSide.Player)
                _subscriptions.Add(eventBus.Subscribe<PlayerAvatarEnergyChangedEvent>(OnPlayerEnergyChanged));
            else
                _subscriptions.Add(eventBus.Subscribe<EnemyAvatarEnergyChangedEvent>(OnEnemyEnergyChanged));

            _subscriptions.Add(_battleActionRuntimeService.State.Subscribe(_ => RefreshReadyState()));
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
            _isEnergyReady = max > 0 && current >= max;
            RefreshReadyState();
        }

        private void RefreshReadyState()
        {
            var canActivate = _battleActionRuntimeService.Evaluate(BattleActionKind.AvatarActivation).IsAllowed;
            IsReady.Value = _isEnergyReady && canActivate;
        }
    }
}