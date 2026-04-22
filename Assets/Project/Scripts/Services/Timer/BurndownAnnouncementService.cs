using System;
using Cysharp.Threading.Tasks;
using Project.Scripts.Services.Announcements;
using Project.Scripts.Services.Events;
using VContainer.Unity;

namespace Project.Scripts.Services.Timer
{
    public class BurndownAnnouncementService : IStartable, IDisposable
    {
        private readonly IBoardAnnouncementService _announcementService;
        private readonly IDisposable _burndownStartedSub;


        public BurndownAnnouncementService(EventBus eventBus, IBoardAnnouncementService announcementService)
        {
            _announcementService = announcementService;
            _burndownStartedSub = eventBus.Subscribe<BurndownStartedEvent>(OnBurndownStarted);
        }


        public void Start()
        {
        }

        public void Dispose()
        {
            _burndownStartedSub.Dispose();
        }


        private void OnBurndownStarted(BurndownStartedEvent _)
        {
            _announcementService.Show("Burndown!").Forget();
        }
    }
}