using System;
using System.Collections.Generic;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared.Avatar;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Tiles;
using VContainer.Unity;

namespace Project.Scripts.Services.Combat
{
    public class EnemyAvatarChargeService : IEnemyAvatarChargeService, IStartable, IDisposable
    {
        public int CurrentEnergy => _engine.Snapshot.CurrentEnergy;
        public int MaxEnergy => _engine.Snapshot.MaxEnergy;
        public bool IsReady => _engine.Snapshot.IsReady;
        public HeroActionType AbilityType { get; }
        public int AbilityPower { get; }


        private readonly EventBus _eventBus;
        private readonly AvatarEnergyEngine _engine = new AvatarEnergyEngine();
        private readonly AvatarEnergyFormula _formula;


        public EnemyAvatarChargeService(EventBus eventBus, LevelConfig levelConfig, SlotLayoutConfig slotLayoutConfig)
        {
            _eventBus = eventBus;
            var config = levelConfig.EnemyAvatarConfig;
            _engine.Initialize(config.MaxEnergy);
            AbilityType = config.AbilityType;
            AbilityPower = config.AbilityPower;
            _formula = new AvatarEnergyFormula(
                slotLayoutConfig.AvatarSlotKind,
                config.PrimaryTileMultiplier,
                config.SecondaryTileMultiplier
            );
        }

        public void Start()
        {
        }

        public void AddEnergy(int amount)
        {
            var added = _engine.TryAddEnergy(amount);
            if (added > 0f)
                PublishEnergyChanged();
        }

        public void AddEnergyFromCascades(IReadOnlyDictionary<TileKind, float> energyByKind)
        {
            var gain = _formula.Calculate(energyByKind);

            var added = _engine.TryAddEnergy(gain);
            if (added <= 0f)
                return;

            PublishEnergyChanged();
        }

        public bool TryRelease()
        {
            var released = _engine.TryRelease();
            if (released <= 0)
                return false;

            PublishEnergyChanged();
            return true;
        }

        public void TriggerAttack()
        {
            var released = _engine.TryRelease();

            if (released <= 0)
                return;

            PublishEnergyChanged();
            _eventBus.Publish(new EnemyAvatarActivatedEvent(AbilityType, AbilityPower));
        }

        public void Dispose() { }


        private void PublishEnergyChanged()
        {
            var snap = _engine.Snapshot;
            _eventBus.Publish(new EnemyAvatarEnergyChangedEvent(snap.CurrentEnergy, snap.MaxEnergy));
        }
    }
}