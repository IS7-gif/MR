using System;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared.Avatar;

namespace Project.Scripts.Services.Combat
{
    public class EnemyAvatarChargeService : IEnemyAvatarChargeService, IDisposable
    {
        public int CurrentEnergy => _engine.Snapshot.CurrentEnergy;
        public int MaxEnergy => _engine.Snapshot.MaxEnergy;
        public bool IsReady => _engine.Snapshot.IsReady;


        private readonly EventBus _eventBus;
        private readonly AvatarEnergyEngine _engine = new AvatarEnergyEngine();


        public EnemyAvatarChargeService(EventBus eventBus, LevelConfig levelConfig)
        {
            _eventBus = eventBus;
            _engine.Initialize(levelConfig.EnemyAvatarConfig.MaxAvatarCharge);
        }

        public void AddEnergy(int amount)
        {
            var added = _engine.TryAddEnergy(amount);

            if (added > 0f)
                PublishEnergyChanged();
        }

        public void TriggerAttack()
        {
            var released = _engine.TryRelease();

            if (released <= 0)
                return;

            PublishEnergyChanged();
            _eventBus.Publish(new EnemyAvatarAttackedEvent(released));
        }

        public void Dispose() { }


        private void PublishEnergyChanged()
        {
            var snap = _engine.Snapshot;
            _eventBus.Publish(new EnemyAvatarEnergyChangedEvent(snap.CurrentEnergy, snap.MaxEnergy));
        }
    }
}