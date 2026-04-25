using System;
using Cysharp.Threading.Tasks;
using Project.Scripts.Services.Announcements;
using Project.Scripts.Services.Events;
using VContainer.Unity;

namespace Project.Scripts.Services.Timer
{
    public class BattlePrePhaseAnnouncementService : IStartable, IDisposable
    {
        private readonly IBoardAnnouncementService _announcementService;
        private readonly IDisposable _startedSub;
        private readonly IDisposable _endedSub;


        public BattlePrePhaseAnnouncementService(EventBus eventBus, IBoardAnnouncementService announcementService)
        {
            _announcementService = announcementService;
            _startedSub = eventBus.Subscribe<BattlePrePhaseStartedEvent>(OnPrePhaseStarted);
            _endedSub = eventBus.Subscribe<BattlePrePhaseEndedEvent>(OnPrePhaseEnded);
        }


        public void Start()
        {
        }

        public void Dispose()
        {
            _startedSub.Dispose();
            _endedSub.Dispose();
        }


        private void OnPrePhaseStarted(BattlePrePhaseStartedEvent _)
        {
            _announcementService.Show("Get Ready!").Forget();
        }

        private void OnPrePhaseEnded(BattlePrePhaseEndedEvent _)
        {
            _announcementService.Show("Go!").Forget();
        }
    }
}