using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Services.Announcements;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared.Timer;
using UnityEngine;
using VContainer.Unity;

namespace Project.Scripts.Services.Timer
{
    public class BattleEscalationService : IStartable, IDisposable
    {
        private readonly BattleTimerConfig _config;
        private readonly EventBus _eventBus;
        private readonly IBoardAnnouncementService _announcementService;
        private readonly List<IBattleEscalationModifier> _modifiers = new();
        private readonly IDisposable _timerSub;
        private bool _escalationTriggered;


        public BattleEscalationService(BattleTimerConfig config, EventBus eventBus, IBoardAnnouncementService announcementService)
        {
            _config = config;
            _eventBus = eventBus;
            _announcementService = announcementService;

            _timerSub = _eventBus.Subscribe<BattleTimerChangedEvent>(OnTimerChanged);
        }


        public void Start()
        {
        }

        public void Dispose()
        {
            _timerSub.Dispose();
        }


        private void OnTimerChanged(BattleTimerChangedEvent e)
        {
            if (_escalationTriggered || false == e.IsEscalation)
                return;

            _escalationTriggered = true;
            _eventBus.Publish(new BattleEscalationReachedEvent(e.TimeRemaining));

            var seconds = Mathf.CeilToInt(e.TimeRemaining);
            _announcementService.Show($"{seconds} seconds!").Forget();

            for (var i = 0; i < _modifiers.Count; i++)
                _modifiers[i].OnEscalationReached();
        }
    }
}