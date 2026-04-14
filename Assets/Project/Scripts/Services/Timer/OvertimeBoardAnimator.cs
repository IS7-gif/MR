using System;
using Cysharp.Threading.Tasks;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Grid;

namespace Project.Scripts.Services.Timer
{
    public class OvertimeBoardAnimator : IDisposable
    {
        private readonly IGridOperations _gridOps;
        private readonly IDisposable _subscription;


        public OvertimeBoardAnimator(EventBus eventBus, IGridOperations gridOps)
        {
            _gridOps = gridOps;
            _subscription = eventBus.Subscribe<OvertimeStartedEvent>(OnOvertimeStarted);
        }


        public void Dispose()
        {
            _subscription.Dispose();
        }


        private void OnOvertimeStarted(OvertimeStartedEvent _)
        {
            _gridOps.CollapseAll().Forget();
        }
    }
}