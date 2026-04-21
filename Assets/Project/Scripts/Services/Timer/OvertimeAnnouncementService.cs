using System;
using Cysharp.Threading.Tasks;
using Project.Scripts.Services.Announcements;
using Project.Scripts.Services.Events;
using VContainer.Unity;

namespace Project.Scripts.Services.Timer
{
    public class OvertimeAnnouncementService : IStartable, IDisposable
    {
        private readonly IBoardAnnouncementService _announcementService;
        private readonly IDisposable _overtimeStartedSub;


        public OvertimeAnnouncementService(EventBus eventBus, IBoardAnnouncementService announcementService)
        {
            _announcementService = announcementService;
            _overtimeStartedSub = eventBus.Subscribe<OvertimeStartedEvent>(OnOvertimeStarted);
        }


        public void Start()
        {
        }

        public void Dispose()
        {
            _overtimeStartedSub.Dispose();
        }


        private void OnOvertimeStarted(OvertimeStartedEvent _)
        {
            _announcementService.Show("Overtime!").Forget();
        }
    }
}