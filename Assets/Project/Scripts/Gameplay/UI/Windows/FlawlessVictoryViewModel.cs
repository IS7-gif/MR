using Cysharp.Threading.Tasks;
using DG.Tweening;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Services.Board;
using Project.Scripts.Services.UISystem;

namespace Project.Scripts.Gameplay.UI
{
    public class FlawlessVictoryViewModel : BaseViewModel
    {
        public float DisplayDuration { get; }
        public float FadeOutDuration { get; }
        public float FlyDistance { get; }
        public Ease FadeOutEase { get; }
        public float BattleAreaCenterWorldY { get; }


        private readonly UniTaskCompletionSource _animationDone = new();


        public FlawlessVictoryViewModel(BattleAnimationConfig animConfig, IBoardBoundsProvider boardBounds)
        {
            DisplayDuration = animConfig.FlawlessDisplayDuration;
            FadeOutDuration = animConfig.FlawlessFadeOutDuration;
            FlyDistance = animConfig.FlawlessFlyDistance;
            FadeOutEase = animConfig.FlawlessFadeOutEase;
            BattleAreaCenterWorldY = boardBounds.BattleAreaCenterWorldY;
        }


        public UniTask WaitAsync() => _animationDone.Task;

        public void NotifyAnimationDone() => _animationDone.TrySetResult();
    }
}
