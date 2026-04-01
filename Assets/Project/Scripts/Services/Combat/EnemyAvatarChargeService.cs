using System;
using Project.Scripts.Configs;
using Project.Scripts.Services.EventBusSystem;
using Project.Scripts.Services.EventBusSystem.Events;
using Project.Scripts.Shared.Avatar;

namespace Project.Scripts.Services.Combat
{
    public class EnemyAvatarChargeService : IEnemyAvatarChargeService, IDisposable
    {
        public int CurrentCharge => _engine.Snapshot.CurrentCharge;
        public int MaxCharge => _engine.Snapshot.MaxCharge;
        public bool IsFull => _engine.Snapshot.IsFull;

        
        private readonly EventBus _eventBus;
        private readonly AvatarChargeEngine _engine = new AvatarChargeEngine();

        
        public EnemyAvatarChargeService(EventBus eventBus, DamageConfig damageConfig)
        {
            _eventBus = eventBus;
            _engine.Initialize(damageConfig.MaxAvatarCharge);
        }

        public void AddCharge(int amount)
        {
            var added = _engine.TryAddCharge(amount);

            if (added > 0)
                PublishChargeChanged();
        }

        public void TriggerDischarge()
        {
            var discharged = _engine.TryDischarge();

            if (discharged <= 0)
                return;

            PublishChargeChanged();
            _eventBus.Publish(new EnemyDischargeEvent(discharged));
        }

        public void Dispose() { }


        private void PublishChargeChanged()
        {
            var snap = _engine.Snapshot;
            _eventBus.Publish(new EnemyChargeChangedEvent(snap.CurrentCharge, snap.MaxCharge));
        }
    }
}