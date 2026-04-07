using System;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared.Avatar;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Project.Scripts.Services.Combat
{
    public class PlayerAvatarChargeService : IPlayerAvatarChargeService, IStartable, IDisposable
    {
        public int CurrentEnergy => _engine.Snapshot.CurrentEnergy;
        public int MaxEnergy => _engine.Snapshot.MaxEnergy;
        public bool IsReady => _engine.Snapshot.IsReady;


        private readonly EventBus _eventBus;
        private readonly AvatarEnergyEngine _engine = new AvatarEnergyEngine();
        private readonly AvatarEnergyFormula _formula;
        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();


        public PlayerAvatarChargeService(EventBus eventBus, LevelConfig levelConfig, SlotLayoutConfig slotLayoutConfig)
        {
            _eventBus = eventBus;
            _engine.Initialize(levelConfig.PlayerAvatarConfig.MaxAvatarCharge);
            _formula = new AvatarEnergyFormula(
                slotLayoutConfig.AvatarSlotKind,
                levelConfig.PlayerAvatarConfig.PrimaryTileMultiplier,
                levelConfig.PlayerAvatarConfig.SecondaryTileMultiplier
            );
        }

        public void Start()
        {
            _subscriptions.Add(_eventBus.Subscribe<EnergyGeneratedEvent>(OnEnergyGenerated));
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }


        public int TryRelease()
        {
            var released = _engine.TryRelease();
            if (released <= 0)
                return 0;

            PublishEnergyChanged();
            return released;
        }


        private void OnEnergyGenerated(EnergyGeneratedEvent e)
        {
            var gain = _formula.Calculate(e.EnergyByKind);
            var added = _engine.TryAddEnergy(gain);

            if (added <= 0f)
                return;

            var snap = _engine.Snapshot;
            Debug.Log($"[PlayerAvatar] +{added:F2} → {snap.CurrentEnergy}/{snap.MaxEnergy}{(snap.IsReady ? " — READY" : string.Empty)}");
            PublishEnergyChanged();
        }

        private void PublishEnergyChanged()
        {
            var snap = _engine.Snapshot;
            _eventBus.Publish(new PlayerAvatarEnergyChangedEvent(snap.CurrentEnergy, snap.MaxEnergy));
        }
    }
}