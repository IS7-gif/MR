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
        private readonly BattleFlowConfig _battleFlowConfig;
        private readonly BoardAnnouncementConfig _announcementConfig;
        private readonly IBoardAnnouncementService _announcementService;
        private readonly IDisposable _timerSub;


        public BattleTimerAnnouncementService(
            BattleFlowConfig battleFlowConfig,
            BoardAnnouncementConfig announcementConfig,
            EventBus eventBus,
            IBoardAnnouncementService announcementService)
        {
            _battleFlowConfig = battleFlowConfig;
            _announcementConfig = announcementConfig;
            _announcementService = announcementService;
            _timerSub = eventBus.Subscribe<BattleFlowCountdownTickEvent>(OnTimerChanged);
        }


        public void Start()
        {
        }

        public void Dispose()
        {
            _timerSub.Dispose();
        }


        private void OnTimerChanged(BattleFlowCountdownTickEvent e)
        {
            TryShowCountdown(e);
        }

        private void TryShowCountdown(BattleFlowCountdownTickEvent e)
        {
            var secondsLeft = e.SecondsRemaining;
            if (secondsLeft <= 0 || secondsLeft > _battleFlowConfig.CountdownThreshold)
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