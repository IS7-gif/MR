using Cysharp.Threading.Tasks;
using DG.Tweening;
using Project.Scripts.Services.UISystem;
using UnityEngine;

namespace Project.Scripts.Gameplay.UI.Windows
{
    public class BoardAnnouncementViewModel : BaseViewModel
    {
        public string Text { get; }
        public Color TextColor { get; }
        public float DisplayDuration { get; }
        public float FadeOutDuration { get; }
        public float FlyDistance { get; }
        public Ease FadeOutEase { get; }
        public float BattleAreaCenterWorldY { get; }


        private readonly UniTaskCompletionSource _animationDone = new();


        public BoardAnnouncementViewModel(
            string text,
            Color textColor,
            float displayDuration,
            float fadeOutDuration,
            float flyDistance,
            Ease fadeOutEase,
            float battleAreaCenterWorldY)
        {
            Text = text;
            TextColor = textColor;
            DisplayDuration = displayDuration;
            FadeOutDuration = fadeOutDuration;
            FlyDistance = flyDistance;
            FadeOutEase = fadeOutEase;
            BattleAreaCenterWorldY = battleAreaCenterWorldY;
        }


        public UniTask WaitAsync()
        {
            return _animationDone.Task;
        }

        public void NotifyAnimationDone()
        {
            _animationDone.TrySetResult();
        }
    }
}