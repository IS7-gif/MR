using System;
using System.Collections.Generic;
using Project.Scripts.Configs;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Game;
using Project.Scripts.Shared.Avatar;
using Project.Scripts.Shared.Rules;
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
        private readonly DebugConfig _debugConfig;
        private readonly IEscalationModifierService _escalationModifier;
        private readonly IBattleActionRuntimeService _battleActionRuntimeService;
        private readonly AvatarEnergyEngine _engine = new AvatarEnergyEngine();
        private readonly AvatarEnergyFormula _formula;
        private readonly float _deadHeroTileMultiplier;
        private readonly HashSet<TileKind> _bonusKinds = new();
        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();


        public PlayerAvatarChargeService(EventBus eventBus, DebugConfig debugConfig, LevelConfig levelConfig,
            SlotLayoutConfig slotLayoutConfig, IEscalationModifierService escalationModifier,
            IBattleActionRuntimeService battleActionRuntimeService)
        {
            _eventBus = eventBus;
            _debugConfig = debugConfig;
            _escalationModifier = escalationModifier;
            _battleActionRuntimeService = battleActionRuntimeService;
            var config = levelConfig.PlayerAvatarConfig;
            _engine.Initialize(config.MaxEnergy);
            AbilityType = config.AbilityType;
            AbilityPower = config.AbilityPower;
            _deadHeroTileMultiplier = config.DeadHeroTileMultiplier;
            _formula = new AvatarEnergyFormula(
                slotLayoutConfig.AvatarSlotKind,
                config.PrimaryTileMultiplier,
                config.SecondaryTileMultiplier
            );
        }

        public void Start()
        {
            _subscriptions.Add(_eventBus.Subscribe<EnergyGeneratedEvent>(OnEnergyGenerated));
            _subscriptions.Add(_eventBus.Subscribe<HeroDefeatedEvent>(OnHeroDefeated));
            _subscriptions.Add(_eventBus.Subscribe<AutoEnergyTickEvent>(OnAutoEnergyTick));
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }


        public bool TryRelease()
        {
            if (false == _battleActionRuntimeService.Evaluate(BattleActionKind.AvatarActivation).IsAllowed)
                return false;

            var released = _engine.TryRelease();
            if (released <= 0)
                return false;

            PublishEnergyChanged();
            
            return true;
        }


        private void OnAutoEnergyTick(AutoEnergyTickEvent e)
        {
            var added = _engine.TryAddEnergy(e.EnergyAmount);
            if (added > 0f)
                PublishEnergyChanged();
        }

        private void OnHeroDefeated(HeroDefeatedEvent e)
        {
            if (e.Side != BattleSide.Player)
                return;

            if (_bonusKinds.Add(e.SlotKind))
                _eventBus.Publish(new AvatarTileBonusActivatedEvent(BattleSide.Player, e.SlotKind));
        }

        private void OnEnergyGenerated(EnergyGeneratedEvent e)
        {
            var gain = _bonusKinds.Count > 0
                ? _formula.Calculate(e.EnergyByKind, _bonusKinds, _deadHeroTileMultiplier)
                : _formula.Calculate(e.EnergyByKind);

            var cascadeMultiplier = _escalationModifier.CascadeEnergyMultiplier;
            gain *= cascadeMultiplier;

            var added = _engine.TryAddEnergy(gain);
            if (added <= 0f)
                return;

            var snap = _engine.Snapshot;
            if (_debugConfig.LogEnergyAccumulation)
                Debug.Log(BuildAvatarEnergyLog(e.EnergyByKind, gain, added, snap.CurrentEnergy, snap.MaxEnergy, snap.IsReady, cascadeMultiplier));
            
            PublishEnergyChanged();
        }

        private string BuildAvatarEnergyLog(IReadOnlyDictionary<TileKind, float> energyByKind, float gain, float added,
            int current, int max, bool isReady, float cascadeMultiplier)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("[PlayerAvatar] == Energy formula ==");

            foreach (var pair in energyByKind)
            {
                if (pair.Value <= 0f)
                    continue;

                var mult = pair.Key == _formula.AvatarKind ? _formula.PrimaryMultiplier : _formula.SecondaryMultiplier;
                var label = pair.Key == _formula.AvatarKind ? "primary" : "secondary";

                if (_bonusKinds.Contains(pair.Key))
                {
                    label += " +dead-hero-bonus";
                    mult *= _deadHeroTileMultiplier;
                }

                sb.AppendLine($"  {pair.Key}: {pair.Value:F2} × {mult:F2} ({label}) = {pair.Value * mult:F2}");
            }

            if (_escalationModifier.IsEscalationActive)
                sb.AppendLine($"  Cascade multiplier: ×{cascadeMultiplier:F2} (escalation)");

            sb.Append($"  Formula={gain:F2}  Really added={added:F2}  Bar={current}/{max}{(isReady ? " - READY" : string.Empty)}");
            return sb.ToString();
        }

        private void PublishEnergyChanged()
        {
            var snap = _engine.Snapshot;
            _eventBus.Publish(new PlayerAvatarEnergyChangedEvent(snap.CurrentEnergy, snap.MaxEnergy));
        }
    }
}