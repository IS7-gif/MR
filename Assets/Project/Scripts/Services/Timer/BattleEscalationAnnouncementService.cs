using System;
using Cysharp.Threading.Tasks;
using Project.Scripts.Services.Announcements;
using Project.Scripts.Services.Events;
using UnityEngine;
using VContainer.Unity;

namespace Project.Scripts.Services.Timer
{
    public class BattleEscalationAnnouncementService : IStartable, IDisposable
    {
        private readonly IBoardAnnouncementService _announcementService;
        private readonly IDisposable _escalationReachedSub;


        public BattleEscalationAnnouncementService(EventBus eventBus, IBoardAnnouncementService announcementService)
        {
            _announcementService = announcementService;
            _escalationReachedSub = eventBus.Subscribe<BattleEscalationReachedEvent>(OnBattleEscalationReached);
        }


        public void Start()
        {
        }

        public void Dispose()
        {
            _escalationReachedSub.Dispose();
        }


        private void OnBattleEscalationReached(BattleEscalationReachedEvent e)
        {
            var seconds = Mathf.CeilToInt(e.TimeRemaining);
            _announcementService.Show($"{seconds} seconds remaining!").Forget();
        }
    }
}