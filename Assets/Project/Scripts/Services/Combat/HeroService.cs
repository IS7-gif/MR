using System;
using System.Collections.Generic;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Game;
using Project.Scripts.Shared.Rules;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Tiles;

namespace Project.Scripts.Services.Combat
{
    public class HeroService : IHeroService, IDisposable
    {
        private const int SlotCount = 4;


        private readonly EventBus _eventBus;
        private readonly IPlayerStateService _playerState;
        private readonly IEnemyStateService _enemyState;
        private readonly IBattleSideEnergyService _battleSideEnergyService;
        private readonly IBattleActionRuntimeService _battleActionRuntimeService;
        private readonly IUnitActivationCooldownService _unitActivationCooldownService;
        private readonly HeroSlotState[] _playerSlots = new HeroSlotState[SlotCount];
        private readonly HeroSlotState[] _enemySlots = new HeroSlotState[SlotCount];


        public HeroService(EventBus eventBus, LevelConfig levelConfig, SlotLayoutConfig slotLayoutConfig,
            IPlayerStateService playerState, IEnemyStateService enemyState, IBattleSideEnergyService battleSideEnergyService,
            IBattleActionRuntimeService battleActionRuntimeService, IUnitActivationCooldownService unitActivationCooldownService)
        {
            _eventBus = eventBus;
            _playerState = playerState;
            _enemyState = enemyState;
            _battleSideEnergyService = battleSideEnergyService;
            _battleActionRuntimeService = battleActionRuntimeService;
            _unitActivationCooldownService = unitActivationCooldownService;

            InitSlots(_playerSlots, levelConfig.PlayerHeroes, slotLayoutConfig.HeroSlotKinds);
            InitSlots(_enemySlots, levelConfig.EnemyHeroes, slotLayoutConfig.HeroSlotKinds);

            PublishInitialHPEvents(_playerSlots, BattleSide.Player);
            PublishInitialHPEvents(_enemySlots, BattleSide.Enemy);
        }


        public IReadOnlyList<HeroSlotState> GetSlots(BattleSide side)
        {
            return side == BattleSide.Player ? _playerSlots : _enemySlots;
        }

        public bool CanActivate(BattleSide side, int slotIndex)
        {
            if (slotIndex is < 0 or >= SlotCount)
                return false;

            ref var slot = ref GetSlotRef(side, slotIndex);
            if (false == slot.IsAssigned || false == slot.IsAlive)
                return false;

            if (slot.ActionType == HeroActionType.HealAlly && IsHpFull(side))
                return false;

            if (_unitActivationCooldownService.IsHeroOnCooldown(side, slotIndex))
                return false;

            return _battleSideEnergyService.CanSpend(side, slot.ActivationEnergyCost);
        }

        public void TryActivate(int slotIndex)
        {
            TryActivate(BattleSide.Player, slotIndex);
        }

        public void TryActivate(BattleSide side, int slotIndex)
        {
            if (false == _battleActionRuntimeService.Evaluate(BattleActionKind.HeroActivation).IsAllowed)
                return;

            if (slotIndex is < 0 or >= SlotCount)
                return;

            if (side == BattleSide.Player)
                TryActivateSlot(ref _playerSlots[slotIndex], BattleSide.Player, slotIndex);
            else
                TryActivateSlot(ref _enemySlots[slotIndex], BattleSide.Enemy, slotIndex);
        }

        public bool TryDischargeHero(BattleSide side, int slotIndex, out HeroActionType actionType, out int actionValue)
        {
            actionType = default;
            actionValue = 0;

            if (false == _battleActionRuntimeService.Evaluate(BattleActionKind.AbilityCommit).IsAllowed)
                return false;

            if (slotIndex is < 0 or >= SlotCount)
                return false;

            ref var slot = ref GetSlotRef(side, slotIndex);

            if (false == CanActivate(side, slotIndex))
                return false;

            actionType = slot.ActionType;
            actionValue = slot.ActionValue;

            if (false == _battleSideEnergyService.TrySpend(side, slot.ActivationEnergyCost))
                return false;

            _unitActivationCooldownService.StartHeroCooldown(side, slotIndex);
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
        }


        private bool IsHpFull(BattleSide side)
        {
            if (side == BattleSide.Player)
                return _playerState.CurrentHP >= _playerState.MaxHP;

            return _enemyState.CurrentHP >= _enemyState.MaxHP;
        }

        private void TryActivateSlot(ref HeroSlotState slot, BattleSide side, int slotIndex)
        {
            if (false == CanActivate(side, slotIndex))
                return;

            if (false == _battleSideEnergyService.TrySpend(side, slot.ActivationEnergyCost))
                return;

            _unitActivationCooldownService.StartHeroCooldown(side, slotIndex);
            _eventBus.Publish(new HeroActivatedEvent(side, slotIndex, slot.ActionType, slot.ActionValue));
        }

        private void ApplyHPChange(ref HeroSlotState slot, BattleSide side, int slotIndex, int delta, bool silent = false)
        {
            if (false == slot.IsAssigned || slot.MaxHP <= 0)
                return;

            var result = HealthChangeRules.Apply(slot.CurrentHP, slot.MaxHP, delta);
            if (false == result.WasChanged)
                return;

            slot.CurrentHP = result.CurrentHP;
            _eventBus.Publish(new HeroHPChangedEvent(side, slotIndex, slot.CurrentHP, slot.MaxHP, silent));

            if (false == result.BecameDefeated)
                return;

            _eventBus.Publish(new HeroDefeatedEvent(side, slotIndex, slot.SlotKind));
        }

        private ref HeroSlotState GetSlotRef(BattleSide side, int slotIndex)
        {
            var slots = side == BattleSide.Player ? _playerSlots : _enemySlots;
            
            return ref slots[slotIndex];
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
                    ActivationEnergyCost = config.ActivationEnergyCost,
                    ActionType = config.AbilityType,
                    ActionValue = config.AbilityPower,
                    CurrentHP = config.MaxHP,
                    MaxHP = config.MaxHP,
                };
            }
        }
    }
}