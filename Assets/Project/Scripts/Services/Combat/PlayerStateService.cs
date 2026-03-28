using System;
using Project.Scripts.Configs;
using Project.Scripts.Services.EventBusSystem;
using Project.Scripts.Services.EventBusSystem.Events;

namespace Project.Scripts.Services.Combat
{
    public class PlayerStateService : IPlayerStateService
    {
        public int CurrentHP { get; private set; }
        public int MaxHP { get; }


        private readonly EventBus _eventBus;


        public PlayerStateService(EventBus eventBus, LevelConfig levelConfig)
        {
            _eventBus = eventBus;
            MaxHP = levelConfig.PlayerHP;
            CurrentHP = levelConfig.PlayerHP;
        }


        public void Heal(int amount)
        {
            if (CurrentHP >= MaxHP || amount <= 0)
                return;

            CurrentHP = Math.Min(MaxHP, CurrentHP + amount);
            _eventBus.Publish(new PlayerHPChangedEvent(CurrentHP, MaxHP));
        }
    }
}