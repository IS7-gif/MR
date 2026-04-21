using System;
using Cysharp.Threading.Tasks;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Configs.UI;
using Project.Scripts.Services.Announcements;
using Project.Scripts.Services.Events;
using VContainer.Unity;

namespace Project.Scripts.Services.Timer
{
    public class BattleTimerAnnouncementService : IStartable, IDisposable
    {
        private readonly BattleTimerConfig _timerConfig;
        private readonly BoardAnnouncementConfig _announcementConfig;
        private readonly IBoardAnnouncementService _announcementService;
        private readonly IDisposable _timerSub;


        public BattleTimerAnnouncementService(
            BattleTimerConfig timerConfig,
            BoardAnnouncementConfig announcementConfig,
            EventBus eventBus,
            IBoardAnnouncementService announcementService)
        {
            _timerConfig = timerConfig;
            _announcementConfig = announcementConfig;
            _announcementService = announcementService;
            _timerSub = eventBus.Subscribe<BattleTimerChangedEvent>(OnTimerChanged);
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
            TryShowCountdown(e);
        }

        private void TryShowCountdown(BattleTimerChangedEvent e)
        {
            var secondsLeft = (int)e.TimeRemaining;
            if (secondsLeft <= 0 || secondsLeft > _timerConfig.CountdownThreshold)
                return;

            var countdownParams = new BoardAnnouncementParams
            {
                TextColor = _announcementConfig.CountdownTextColor,
                DisplayDuration = _announcementConfig.CountdownDisplayDuration,
                FadeOutDuration = _announcementConfig.CountdownFadeOutDuration,
                FlyDistance = _announcementConfig.CountdownFlyDistance,
                FadeOutEase = _announcementConfig.CountdownFadeOutEase
            };

            _announcementService.Show(secondsLeft.ToString(), countdownParams).Forget();
        }

    }
}