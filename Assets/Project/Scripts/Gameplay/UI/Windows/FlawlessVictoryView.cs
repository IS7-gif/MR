using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Project.Scripts.Services.UISystem;
using UnityEngine;

namespace Project.Scripts.Gameplay.UI.Windows
{
    public class FlawlessVictoryView : BaseView<FlawlessVictoryViewModel>
    {
        [Tooltip("CanvasGroup корневого объекта - используется для fade-out")]
        [SerializeField] private CanvasGroup _canvasGroup;

        [Tooltip("RectTransform контейнера текста - используется для анимации полёта вверх")]
        [SerializeField] private RectTransform _textRect;


        private Sequence _sequence;
        private RectTransform _canvasRect;
        private Camera _cam;


        private void Awake()
        {
            _canvasRect = transform.parent as RectTransform;
            _cam = Camera.main;
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }

        protected override async UniTask OnShow()
        {
            if (_canvasGroup)
                _canvasGroup.alpha = 1f;

            var startPos = WorldYToAnchored(ViewModel.BattleAreaCenterWorldY);

            if (_textRect)
                _textRect.anchoredPosition = startPos;

            await UniTask.Delay(TimeSpan.FromSeconds(ViewModel.DisplayDuration));

            if (ViewModel.FadeOutDuration > 0f)
            {
                _sequence?.Kill();
                _sequence = DOTween.Sequence();

                if (_textRect)
                    _sequence.Join(_textRect
                        .DOAnchorPosY(startPos.y + ViewModel.FlyDistance, ViewModel.FadeOutDuration)
                        .SetEase(ViewModel.FadeOutEase));

                if (_canvasGroup)
                    _sequence.Join(_canvasGroup
                        .DOFade(0f, ViewModel.FadeOutDuration)
                        .SetEase(ViewModel.FadeOutEase));

                await _sequence.ToUniTask();
            }

            ViewModel.NotifyAnimationDone();
        }

        private Vector2 WorldYToAnchored(float worldY)
        {
            if (!_cam || !_canvasRect)
                return Vector2.zero;

            var screenPoint = _cam.WorldToScreenPoint(new Vector3(0f, worldY, 0f));
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect, screenPoint, null, out var localPoint);
            
            return localPoint;
        }
    }
}