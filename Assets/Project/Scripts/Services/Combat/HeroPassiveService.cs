using System;
using System.Collections.Generic;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared.BattleFlow;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Passives;
using Project.Scripts.Shared.Tiles;
using VContainer.Unity;

namespace Project.Scripts.Services.Combat
{
    public class HeroPassiveService : IHeroPassiveService, IEnergyGainModifierService, IHeroAbilityModifierService, IStartable, IDisposable
    {
        private const int SlotCount = 4;
        
        
        public IReadOnlyList<HeroPassiveRuntimeState> States => _engine.States;


        private readonly EventBus _eventBus;
        private readonly LevelConfig _levelConfig;
        private readonly SlotLayoutConfig _slotLayoutConfig;
        private readonly PassiveAbilityEngine _engine = new();
        private IDisposable _heroDefeatedSubscription;
        private IDisposable _heroActivatedSubscription;
        private IDisposable _abilityExecutedSubscription;
        private IDisposable _phaseChangedSubscription;
        private BattlePhaseKind _currentPhase = BattlePhaseKind.Match;
        private readonly bool[,] _slotKindPassiveStates = new bool[2, SlotCount];


        public HeroPassiveService(EventBus eventBus, LevelConfig levelConfig, SlotLayoutConfig slotLayoutConfig)
        {
            _eventBus = eventBus;
            _levelConfig = levelConfig;
            _slotLayoutConfig = slotLayoutConfig;
        }


        public void Start()
        {
            InitializePassives();
            _heroDefeatedSubscription = _eventBus.Subscribe<HeroDefeatedEvent>(OnHeroDefeated);
            _heroActivatedSubscription = _eventBus.Subscribe<HeroActivatedEvent>(OnHeroActivated);
            _abilityExecutedSubscription = _eventBus.Subscribe<AbilityExecutedEvent>(OnAbilityExecuted);
            _phaseChangedSubscription = _eventBus.Subscribe<BattleFlowPhaseChangedEvent>(OnBattleFlowPhaseChanged);
        }

        public void Dispose()
        {
            _heroDefeatedSubscription?.Dispose();
            _heroDefeatedSubscription = null;
            _heroActivatedSubscription?.Dispose();
            _heroActivatedSubscription = null;
            _abilityExecutedSubscription?.Dispose();
            _abilityExecutedSubscription = null;
            _phaseChangedSubscription?.Dispose();
            _phaseChangedSubscription = null;
        }

        public float CalculateMatchEnergy(BattleSide side, IReadOnlyDictionary<TileKind, float> energyByKind)
        {
            return PassiveEnergyRules.SumMatchEnergyWithPassives(energyByKind, _engine.States, side);
        }

        public int GetActivationEnergyCost(BattleSide side, int slotIndex, int baseCost)
        {
            return PassiveHeroAbilityRules.GetModifiedActivationEnergyCost(baseCost, _engine.States, side, slotIndex);
        }

        public int GetAbilityPower(BattleSide side, int slotIndex, int basePower)
        {
            return PassiveHeroAbilityRules.GetModifiedAbilityPower(basePower, _engine.States, side, slotIndex);
        }


        private void InitializePassives()
        {
            var setups = new List<HeroPassiveSetup>();
            AddSidePassives(setups, BattleSide.Player, _levelConfig.PlayerHeroes);
            AddSidePassives(setups, BattleSide.Enemy, _levelConfig.EnemyHeroes);
            _engine.Initialize(setups);
        }

        private void AddSidePassives(List<HeroPassiveSetup> setups, BattleSide side, HeroConfig[] heroes)
        {
            if (heroes == null)
                return;

            for (var slotIndex = 0; slotIndex < SlotCount && slotIndex < heroes.Length; slotIndex++)
            {
                var hero = heroes[slotIndex];
                if (!hero)
                    continue;

                var passiveConfigs = hero.PassiveAbilities;
                if (passiveConfigs == null || passiveConfigs.Length == 0)
                    continue;

                var slotKind = GetSlotKind(slotIndex);
                for (var passiveIndex = 0; passiveIndex < passiveConfigs.Length; passiveIndex++)
                {
                    var passiveConfig = passiveConfigs[passiveIndex];
                    if (passiveConfig == null)
                        continue;

                    var definition = passiveConfig.ToDefinition();
                    if (false == definition.IsConfigured)
                        continue;

                    setups.Add(new HeroPassiveSetup(side, slotIndex, slotKind, definition));
                }
            }
        }

        private TileKind GetSlotKind(int slotIndex)
        {
            var slotKinds = _slotLayoutConfig.HeroSlotKinds;
            return slotKinds != null && slotIndex >= 0 && slotIndex < slotKinds.Length
                ? slotKinds[slotIndex] : TileKind.None;
        }

        private void OnHeroDefeated(HeroDefeatedEvent e)
        {
            if (_engine.DisableOwner(e.Side, e.SlotIndex))
            {
                _eventBus.Publish(new HeroPassiveDisabledEvent(e.Side, e.SlotIndex));
                RefreshSlotKindPassiveState(e.Side, e.SlotIndex);
            }
        }

        private void OnHeroActivated(HeroActivatedEvent e)
        {
            if (_currentPhase != BattlePhaseKind.Hero)
                return;

            AddProgressAndPublishActivations(PassiveConditionKind.HeroActivationsInHeroPhase, e.Side, 1, e.SlotIndex);
        }

        private void OnAbilityExecuted(AbilityExecutedEvent e)
        {
            if (_currentPhase != BattlePhaseKind.Hero || e.Source.Kind != UnitKind.Hero)
                return;

            AddProgressAndPublishActivations(PassiveConditionKind.HeroActivationsInHeroPhase, e.Source.Side, 1, e.Source.SlotIndex);
        }

        private void OnBattleFlowPhaseChanged(BattleFlowPhaseChangedEvent e)
        {
            _currentPhase = e.Phase;

            if (e.Phase == BattlePhaseKind.Hero)
            {
                _engine.ResetConditionProgress(PassiveConditionKind.HeroActivationsInHeroPhase, BattleSide.Player);
                _engine.ResetConditionProgress(PassiveConditionKind.HeroActivationsInHeroPhase, BattleSide.Enemy);
            }
        }

        private void AddProgressAndPublishActivations(PassiveConditionKind conditionKind, BattleSide side, int amount, int slotIndex)
        {
            var activationCounts = CaptureActivationCounts();
            if (false == _engine.AddConditionProgress(conditionKind, side, amount, slotIndex))
                return;

            PublishNewActivations(activationCounts);
        }

        private int[] CaptureActivationCounts()
        {
            var states = _engine.States;
            var result = new int[states.Count];
            for (var i = 0; i < states.Count; i++)
                result[i] = states[i].ActivationCount;
            
            return result;
        }

        private void PublishNewActivations(int[] previousActivationCounts)
        {
            var states = _engine.States;
            for (var i = 0; i < states.Count && i < previousActivationCounts.Length; i++)
            {
                if (states[i].ActivationCount <= previousActivationCounts[i])
                    continue;

                _eventBus.Publish(new HeroPassiveActivatedEvent(states[i]));
                PublishHeroAbilityStatsChanged(states[i].Side, states[i].SlotIndex);
                RefreshSlotKindPassiveState(states[i].Side, states[i].SlotIndex);
            }
        }

        private void PublishHeroAbilityStatsChanged(BattleSide side, int slotIndex)
        {
            var heroConfig = GetHeroConfig(side, slotIndex);
            if (!heroConfig)
                return;

            _eventBus.Publish(new HeroAbilityStatsChangedEvent(side, slotIndex,
                GetActivationEnergyCost(side, slotIndex, heroConfig.ActivationEnergyCost),
                GetAbilityPower(side, slotIndex, heroConfig.AbilityPower)));
        }

        private HeroConfig GetHeroConfig(BattleSide side, int slotIndex)
        {
            if (slotIndex is < 0 or >= SlotCount)
                return null;

            var heroes = side == BattleSide.Player
                ? _levelConfig.PlayerHeroes
                : _levelConfig.EnemyHeroes;

            return heroes != null && slotIndex < heroes.Length ? heroes[slotIndex] : null;
        }

        private void RefreshSlotKindPassiveState(BattleSide side, int slotIndex)
        {
            if (slotIndex is < 0 or >= SlotCount)
                return;

            var sideIndex = GetSideIndex(side);
            var active = HasActiveSlotKindPassive(side, slotIndex);
            if (_slotKindPassiveStates[sideIndex, slotIndex] == active)
                return;

            _slotKindPassiveStates[sideIndex, slotIndex] = active;
            _eventBus.Publish(new HeroSlotKindPassiveStateChangedEvent(side, slotIndex, active));
        }

        private bool HasActiveSlotKindPassive(BattleSide side, int slotIndex)
        {
            var states = _engine.States;
            for (var i = 0; i < states.Count; i++)
            {
                var state = states[i];
                if (state.Side != side || state.SlotIndex != slotIndex)
                    continue;

                if (false == state.IsActive || state.IsDisabled)
                    continue;

                if (PassiveAbilityRules.HasSlotKindLinkedModifier(state.Definition))
                    return true;
            }

            return false;
        }

        private static int GetSideIndex(BattleSide side)
        {
            return side == BattleSide.Player ? 0 : 1;
        }
    }
}