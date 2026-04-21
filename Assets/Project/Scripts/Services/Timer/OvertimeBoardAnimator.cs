using System;
using Cysharp.Threading.Tasks;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Grid;
using VContainer.Unity;

namespace Project.Scripts.Services.Timer
{
    public class OvertimeBoardAnimator : IStartable, IDisposable
    {
        private readonly OvertimeConfig _config;
        private readonly IGridOperations _gridOps;
        private readonly IDisposable _subscription;


        public OvertimeBoardAnimator(EventBus eventBus, OvertimeConfig config, IGridOperations gridOps)
        {
            _config = config;
            _gridOps = gridOps;
            _subscription = eventBus.Subscribe<OvertimeStartedEvent>(OnOvertimeStarted);
        }


        public void Start()
        {
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }


        private void OnOvertimeStarted(OvertimeStartedEvent _)
        {
            _gridOps.CollapseAll(_config.CollapseAllDuration, _config.CollapseAllEase).Forget();
        }
    }
}