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
        private readonly BattleTimerConfig _timerConfig;
        private readonly BattleAnimationConfig _animConfig;
        private readonly EventBus _eventBus;
        private readonly IBoardAnnouncementService _announcementService;

        private readonly List<IBattleEscalationModifier> _modifiers;
        private readonly IDisposable _timerSub;
        private bool _escalationTriggered;


        public BattleEscalationService(
            BattleTimerConfig timerConfig,
            BattleAnimationConfig animConfig,
            EventBus eventBus,
            IBoardAnnouncementService announcementService,
            IEnumerable<IBattleEscalationModifier> modifiers)
        {
            _timerConfig = timerConfig;
            _animConfig = animConfig;
            _eventBus = eventBus;
            _announcementService = announcementService;
            _modifiers = new List<IBattleEscalationModifier>(modifiers);

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
            TryTriggerEscalation(e);
            TryShowCountdown(e);
        }

        private void TryTriggerEscalation(BattleTimerChangedEvent e)
        {
            if (_escalationTriggered || false == e.IsEscalation)
                return;

            _escalationTriggered = true;
            _eventBus.Publish(new BattleEscalationReachedEvent(e.TimeRemaining));

            var seconds = Mathf.CeilToInt(e.TimeRemaining);
            _announcementService.Show($"{seconds} seconds remaining!").Forget();

            for (var i = 0; i < _modifiers.Count; i++)
                _modifiers[i].OnEscalationReached();
        }

        private void TryShowCountdown(BattleTimerChangedEvent e)
        {
            var secondsLeft = (int)e.TimeRemaining;

            if (secondsLeft <= 0 || secondsLeft > _timerConfig.CountdownThreshold)
                return;

            var countdownParams = new BoardAnnouncementParams
            {
                TextColor = _animConfig.CountdownTextColor,
                DisplayDuration = _animConfig.CountdownDisplayDuration,
                FadeOutDuration = _animConfig.CountdownFadeOutDuration,
                FlyDistance = _animConfig.CountdownFlyDistance,
                FadeOutEase = _animConfig.CountdownFadeOutEase
            };

            _announcementService.Show(secondsLeft.ToString(), countdownParams).Forget();
        }
    }
}