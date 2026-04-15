using System;
using System.Collections.Generic;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Tiles;
using R3;

namespace Project.Scripts.Services.Combat
{
    public class HeroService : IHeroService, IDisposable
    {
        private const int SlotCount = 4;


        private readonly EventBus _eventBus;
        private readonly IPlayerStateService _playerState;
        private readonly IEnemyStateService _enemyState;
        private readonly IEscalationModifierService _escalationModifier;
        private readonly HeroSlotState[] _playerSlots = new HeroSlotState[SlotCount];
        private readonly HeroSlotState[] _enemySlots = new HeroSlotState[SlotCount];
        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();


        public HeroService(EventBus eventBus, LevelConfig levelConfig, SlotLayoutConfig slotLayoutConfig,
            IPlayerStateService playerState, IEnemyStateService enemyState, IEscalationModifierService escalationModifier)
        {
            _eventBus = eventBus;
            _playerState = playerState;
            _enemyState = enemyState;
            _escalationModifier = escalationModifier;

            InitSlots(_playerSlots, levelConfig.PlayerHeroes, slotLayoutConfig.HeroSlotKinds);
            InitSlots(_enemySlots, levelConfig.EnemyHeroes, slotLayoutConfig.HeroSlotKinds);

            PublishInitialHPEvents(_playerSlots, BattleSide.Player);
            PublishInitialHPEvents(_enemySlots, BattleSide.Enemy);

            _subscriptions.Add(_eventBus.Subscribe<EnergyGeneratedEvent>(OnEnergyGenerated));
            _subscriptions.Add(_eventBus.Subscribe<AutoEnergyTickEvent>(OnAutoEnergyTick));
        }


        public IReadOnlyList<HeroSlotState> GetSlots(BattleSide side)
        {
            return side == BattleSide.Player ? _playerSlots : _enemySlots;
        }

        public void TryActivate(int slotIndex)
        {
            TryActivate(BattleSide.Player, slotIndex);
        }

        public void TryActivate(BattleSide side, int slotIndex)
        {
            if (slotIndex is < 0 or >= SlotCount)
                return;

            if (side == BattleSide.Player)
                TryActivateSlot(ref _playerSlots[slotIndex], BattleSide.Player, slotIndex);
            else
                TryActivateSlot(ref _enemySlots[slotIndex], BattleSide.Enemy, slotIndex);
        }

        public void AddEnemyHeroEnergy(int slotIndex, int amount)
        {
            if (slotIndex is < 0 or >= SlotCount)
                return;

            ref var slot = ref _enemySlots[slotIndex];

            if (!slot.CanAccumulateEnergy || amount <= 0)
                return;

            slot.CurrentEnergy = MathF.Min(slot.MaxEnergy, slot.CurrentEnergy + amount);
            _eventBus.Publish(new HeroEnergyChangedEvent(
                BattleSide.Enemy, slotIndex, (int)slot.CurrentEnergy, slot.MaxEnergy));
        }

        public bool TryDischargeHero(BattleSide side, int slotIndex, out HeroActionType actionType, out int actionValue)
        {
            actionType = default;
            actionValue = 0;

            if (slotIndex is < 0 or >= SlotCount)
                return false;

            ref var slot = ref GetSlotRef(side, slotIndex);

            if (!slot.IsReady)
                return false;

            actionType = slot.ActionType;
            actionValue = slot.ActionValue;

            slot.CurrentEnergy = 0f;
            _eventBus.Publish(new HeroEnergyChangedEvent(side, slotIndex, 0, slot.MaxEnergy));

            return true;
        }

        public void ApplyDamageToHero(BattleSide side, int slotIndex, int amount, bool silent = false)
        {
            if (slotIndex is < 0 or >= SlotCount || amount <= 0)
                return;

            ref var slot = ref GetSlotRef(side, slotIndex);
            ApplyHPChange(ref slot, side, slotIndex, -amount, silent);
        }

        public void ApplyHealToHero(BattleSide side, int slotIndex, int amount)
        {
            if (slotIndex is < 0 or >= SlotCount || amount <= 0)
                return;

            ref var slot = ref GetSlotRef(side, slotIndex);
            ApplyHPChange(ref slot, side, slotIndex, +amount);
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }


        private bool IsHpFull(BattleSide side)
        {
            if (side == BattleSide.Player)
                return _playerState.CurrentHP >= _playerState.MaxHP;

            return _enemyState.CurrentHP >= _enemyState.MaxHP;
        }

        private void TryActivateSlot(ref HeroSlotState slot, BattleSide side, int slotIndex)
        {
            if (false == slot.IsReady)
                return;

            if (slot.ActionType == HeroActionType.HealAlly && IsHpFull(side))
                return;

            _eventBus.Publish(new HeroActivatedEvent(side, slotIndex, slot.ActionType, slot.ActionValue));

            slot.CurrentEnergy = 0f;
            _eventBus.Publish(new HeroEnergyChangedEvent(side, slotIndex, 0, slot.MaxEnergy));
        }

        private void ApplyHPChange(ref HeroSlotState slot, BattleSide side, int slotIndex, int delta, bool silent = false)
        {
            if (false == slot.IsAssigned || slot.MaxHP <= 0)
                return;

            if (false == slot.IsAlive)
                return;

            slot.CurrentHP = Math.Clamp(slot.CurrentHP + delta, 0, slot.MaxHP);
            _eventBus.Publish(new HeroHPChangedEvent(side, slotIndex, slot.CurrentHP, slot.MaxHP, silent));

            if (slot.CurrentHP > 0)
                return;

            slot.CurrentEnergy = 0f;
            _eventBus.Publish(new HeroEnergyChangedEvent(side, slotIndex, 0, slot.MaxEnergy));
            _eventBus.Publish(new HeroDefeatedEvent(side, slotIndex, slot.SlotKind));
        }

        private ref HeroSlotState GetSlotRef(BattleSide side, int slotIndex)
        {
            var slots = side == BattleSide.Player ? _playerSlots : _enemySlots;
            
            return ref slots[slotIndex];
        }

        private void OnAutoEnergyTick(AutoEnergyTickEvent e)
        {
            DistributeAutoTickToSlots(_playerSlots, BattleSide.Player, e.EnergyAmount);
            DistributeAutoTickToSlots(_enemySlots, BattleSide.Enemy, e.EnergyAmount);
        }

        private void DistributeAutoTickToSlots(HeroSlotState[] slots, BattleSide side, float amount)
        {
            for (var i = 0; i < slots.Length; i++)
            {
                ref var slot = ref slots[i];

                if (!slot.CanAccumulateEnergy)
                    continue;

                slot.CurrentEnergy = MathF.Min(slot.MaxEnergy, slot.CurrentEnergy + amount);
                _eventBus.Publish(new HeroEnergyChangedEvent(side, i, (int)slot.CurrentEnergy, slot.MaxEnergy));
            }
        }

        private void OnEnergyGenerated(EnergyGeneratedEvent e)
        {
            var cascadeMultiplier = _escalationModifier.CascadeEnergyMultiplier;
            foreach (var pair in e.EnergyByKind)
                DistributeToSlots(_playerSlots, BattleSide.Player, pair.Key, pair.Value * cascadeMultiplier);
        }

        private void DistributeToSlots(HeroSlotState[] slots, BattleSide side, TileKind kind, float amount)
        {
            for (var i = 0; i < slots.Length; i++)
            {
                ref var slot = ref slots[i];

                if (!slot.CanAccumulateEnergy || slot.SlotKind != kind)
                    continue;

                slot.CurrentEnergy = MathF.Min(slot.MaxEnergy, slot.CurrentEnergy + amount);
                _eventBus.Publish(new HeroEnergyChangedEvent(side, i, (int)slot.CurrentEnergy, slot.MaxEnergy));
            }
        }

        private void PublishInitialHPEvents(HeroSlotState[] slots, BattleSide side)
        {
            for (var i = 0; i < slots.Length; i++)
            {
                if (false == slots[i].IsAssigned)
                    continue;

                _eventBus.Publish(new HeroHPChangedEvent(side, i, slots[i].CurrentHP, slots[i].MaxHP));
            }
        }

        private static void InitSlots(HeroSlotState[] slots, HeroConfig[] configs, TileKind[] slotKinds)
        {
            for (var i = 0; i < SlotCount; i++)
            {
                var slotKind = (slotKinds != null && i < slotKinds.Length) ? slotKinds[i] : default;
                var config = (configs != null && i < configs.Length) ? configs[i] : null;
                if (!config)
                {
                    slots[i] = new HeroSlotState { SlotKind = slotKind };
                    continue;
                }

                slots[i] = new HeroSlotState
                {
                    SlotKind = slotKind,
                    IsAssigned = true,
                    CurrentEnergy = 0f,
                    MaxEnergy = config.MaxEnergy,
                    ActionType = config.AbilityType,
                    ActionValue = config.AbilityPower,
                    CurrentHP = config.MaxHP,
                    MaxHP = config.MaxHP,
                };
            }
        }
    }
}