using System;
using System.Collections.Generic;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Game;
using Project.Scripts.Shared.Avatar;
using Project.Scripts.Shared.Rules;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Tiles;
using R3;
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
        private readonly IEscalationModifierService _escalationModifier;
        private readonly IGameStateService _gameStateService;
        private readonly IBattleActionRuntimeService _battleActionRuntimeService;
        private readonly AvatarEnergyEngine _engine = new AvatarEnergyEngine();
        private readonly AvatarEnergyFormula _formula;
        private readonly float _deadHeroTileMultiplier;
        private readonly HashSet<TileKind> _bonusKinds = new();
        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();


        public EnemyAvatarChargeService(EventBus eventBus, LevelConfig levelConfig, SlotLayoutConfig slotLayoutConfig,
            IEscalationModifierService escalationModifier, IGameStateService gameStateService,
            IBattleActionRuntimeService battleActionRuntimeService)
        {
            _eventBus = eventBus;
            _escalationModifier = escalationModifier;
            _gameStateService = gameStateService;
            _battleActionRuntimeService = battleActionRuntimeService;
            var config = levelConfig.EnemyAvatarConfig;
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
            _subscriptions.Add(_eventBus.Subscribe<HeroDefeatedEvent>(OnHeroDefeated));
            _subscriptions.Add(_eventBus.Subscribe<AutoEnergyTickEvent>(OnAutoEnergyTick));
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }


        public void AddEnergy(int amount)
        {
            var added = _engine.TryAddEnergy(amount);
            if (added > 0f)
                PublishEnergyChanged();
        }

        public void AddEnergyFromCascades(IReadOnlyDictionary<TileKind, float> energyByKind)
        {
            var gain = _bonusKinds.Count > 0
                ? _formula.Calculate(energyByKind, _bonusKinds, _deadHeroTileMultiplier)
                : _formula.Calculate(energyByKind);

            gain *= _escalationModifier.CascadeEnergyMultiplier;

            var added = _engine.TryAddEnergy(gain);
            if (added <= 0f)
                return;

            PublishEnergyChanged();
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

        public void TriggerAttack()
        {
            if (false == _battleActionRuntimeService.Evaluate(BattleActionKind.AvatarActivation).IsAllowed)
                return;

            var released = _engine.TryRelease();

            if (released <= 0)
                return;

            PublishEnergyChanged();
            _eventBus.Publish(new EnemyAvatarActivatedEvent(AbilityType, AbilityPower));
        }


        private void OnAutoEnergyTick(AutoEnergyTickEvent e)
        {
            if (false == _gameStateService.IsPlaying)
                return;

            var added = _engine.TryAddEnergy(e.EnergyAmount);
            if (added > 0f)
                PublishEnergyChanged();
        }

        private void OnHeroDefeated(HeroDefeatedEvent e)
        {
            if (e.Side != BattleSide.Enemy)
                return;

            if (_bonusKinds.Add(e.SlotKind))
                _eventBus.Publish(new AvatarTileBonusActivatedEvent(BattleSide.Enemy, e.SlotKind));
        }

        private void PublishEnergyChanged()
        {
            var snap = _engine.Snapshot;
            _eventBus.Publish(new EnemyAvatarEnergyChangedEvent(snap.CurrentEnergy, snap.MaxEnergy));
        }
    }
}