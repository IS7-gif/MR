using System;
using System.Collections.Generic;
using Project.Scripts.Configs;
using Project.Scripts.Services.EventBusSystem;
using Project.Scripts.Services.EventBusSystem.Events;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Tiles;

namespace Project.Scripts.Services.Combat
{
    public class HeroService : IHeroService, IDisposable
    {
        private const int SlotCount = 4;


        private readonly EventBus _eventBus;
        private readonly IPlayerStateService _playerState;
        private readonly HeroSlotState[] _playerSlots = new HeroSlotState[SlotCount];
        private readonly HeroSlotState[] _enemySlots = new HeroSlotState[SlotCount];
        private IDisposable _subscription;


        public HeroService(EventBus eventBus, LevelConfig levelConfig, IPlayerStateService playerState)
        {
            _eventBus = eventBus;
            _playerState = playerState;

            InitSlots(_playerSlots, levelConfig.PlayerHeroes);
            InitSlots(_enemySlots, levelConfig.EnemyHeroes);

            _subscription = _eventBus.Subscribe<EnergyGeneratedEvent>(OnEnergyGenerated);
        }


        public IReadOnlyList<HeroSlotState> GetSlots(BattleSide side) =>
            side == BattleSide.Player ? _playerSlots : _enemySlots;

        public void TryActivate(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotCount)
                return;

            ref var slot = ref _playerSlots[slotIndex];

            if (false == slot.IsReady)
                return;

            _eventBus.Publish(new HeroActivatedEvent(BattleSide.Player, slotIndex, slot.ActionType, slot.ActionValue));

            if (slot.ActionType == HeroActionType.HealAlly)
                _playerState.Heal(slot.ActionValue);

            slot.CurrentEnergy = 0;
            _eventBus.Publish(new HeroEnergyChangedEvent(BattleSide.Player, slotIndex, 0, slot.MaxEnergy));
        }

        public void Dispose()
        {
            _subscription?.Dispose();
            _subscription = null;
        }


        private void OnEnergyGenerated(EnergyGeneratedEvent e)
        {
            foreach (var pair in e.EnergyByKind)
                DistributeToSlots(_playerSlots, BattleSide.Player, pair.Key, pair.Value);
        }

        private void DistributeToSlots(HeroSlotState[] slots, BattleSide side, TileKind kind, int amount)
        {
            HeroEnergyDistributor.Distribute(slots, kind, amount);

            for (var i = 0; i < slots.Length; i++)
            {
                if (false == slots[i].IsAssigned || slots[i].Kind != kind)
                    continue;

                _eventBus.Publish(new HeroEnergyChangedEvent(side, i, slots[i].CurrentEnergy, slots[i].MaxEnergy));
            }
        }


        private static void InitSlots(HeroSlotState[] slots, HeroConfig[] configs)
        {
            for (var i = 0; i < SlotCount; i++)
            {
                var config = (configs != null && i < configs.Length) ? configs[i] : null;

                if (!config)
                {
                    slots[i] = default;
                    continue;
                }

                slots[i] = new HeroSlotState
                {
                    IsAssigned = true,
                    Kind = config.Kind,
                    CurrentEnergy = 0,
                    MaxEnergy = config.MaxEnergy,
                    ActionType = config.ActionType,
                    ActionValue = config.ActionValue
                };
            }
        }
    }
}