using System;
using System.Collections.Generic;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared.Avatar;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Tiles;
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
        public HeroActionType AbilityType { get; }
        public int AbilityPower { get; }


        private readonly EventBus _eventBus;
        private readonly AvatarEnergyEngine _engine = new AvatarEnergyEngine();
        private readonly AvatarEnergyFormula _formula;
        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();


        public PlayerAvatarChargeService(EventBus eventBus, LevelConfig levelConfig, SlotLayoutConfig slotLayoutConfig)
        {
            _eventBus = eventBus;
            var config = levelConfig.PlayerAvatarConfig;
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
            _subscriptions.Add(_eventBus.Subscribe<EnergyGeneratedEvent>(OnEnergyGenerated));
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }


        public bool TryRelease()
        {
            var released = _engine.TryRelease();
            if (released <= 0)
                return false;

            PublishEnergyChanged();
            return true;
        }


        private void OnEnergyGenerated(EnergyGeneratedEvent e)
        {
            var gain = _formula.Calculate(e.EnergyByKind);
            var added = _engine.TryAddEnergy(gain);
            if (added <= 0f)
                return;

            var snap = _engine.Snapshot;
            Debug.Log(BuildAvatarEnergyLog(e.EnergyByKind, gain, added, snap.CurrentEnergy, snap.MaxEnergy, snap.IsReady));
            PublishEnergyChanged();
        }

        private string BuildAvatarEnergyLog(IReadOnlyDictionary<TileKind, float> energyByKind, float gain, float added, int current, int max, bool isReady)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("[PlayerAvatar] == Energy formula ==");

            foreach (var pair in energyByKind)
            {
                if (pair.Value <= 0f)
                    continue;

                var mult = pair.Key == _formula.AvatarKind ? _formula.PrimaryMultiplier : _formula.SecondaryMultiplier;
                var label = pair.Key == _formula.AvatarKind ? "primary" : "secondary";
                sb.AppendLine($"  {pair.Key}: {pair.Value:F2} × {mult:F2} ({label}) = {pair.Value * mult:F2}");
            }

            sb.Append($"  Gain={gain:F2}  Added={added:F2}  Charge={current}/{max}{(isReady ? " - READY" : string.Empty)}");
            return sb.ToString();
        }

        private void PublishEnergyChanged()
        {
            var snap = _engine.Snapshot;
            _eventBus.Publish(new PlayerAvatarEnergyChangedEvent(snap.CurrentEnergy, snap.MaxEnergy));
        }
    }
}