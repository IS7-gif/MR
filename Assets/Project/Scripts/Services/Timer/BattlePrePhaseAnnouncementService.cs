using System;
using Cysharp.Threading.Tasks;
using Project.Scripts.Services.Announcements;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared.BattleFlow;
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


        private void OnPrePhaseStarted(BattlePrePhaseStartedEvent e)
        {
            _announcementService.Show("Get Ready!", new BoardAnnouncementParams
            {
                Anchor = PhaseToAnchor(e.UpcomingPhase)
            }).Forget();
        }

        private void OnPrePhaseEnded(BattlePrePhaseEndedEvent e)
        {
            _announcementService.Show("Go!", new BoardAnnouncementParams
            {
                Anchor = PhaseToAnchor(e.NextPhase)
            }).Forget();
        }


        private static AnnouncementAnchorKind PhaseToAnchor(BattlePhaseKind phase)
        {
            return phase == BattlePhaseKind.Match
                ? AnnouncementAnchorKind.Board
                : AnnouncementAnchorKind.BattleField;
        }
    }
}
