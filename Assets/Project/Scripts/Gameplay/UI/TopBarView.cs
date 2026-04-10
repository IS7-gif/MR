using Cysharp.Threading.Tasks;
using Project.Scripts.Gameplay.Battle.HUD;
using Project.Scripts.Services.UISystem;
using R3;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.UI
{
    public class TopBarView : BaseView<BattleHUDViewModel>
    {
        [Tooltip("Корневой RectTransform верхнего контейнера - получает смещение по Y safe area во время выполнения")]
        [SerializeField] private RectTransform _topContainer;

        [Tooltip("Отображает имя противника")]
        [SerializeField] private TMP_Text _enemyNameTMP;

        [Tooltip("таймер боя")]
        [SerializeField] private TMP_Text _timerTMP;


        private Canvas _canvas;


        protected override UniTask OnBindViewModel()
        {
            _canvas = GetComponentInParent<Canvas>();

            if (_enemyNameTMP)
                _enemyNameTMP.text = ViewModel.EnemyName;

            Disposables.Add(ViewModel.TimerSeconds.Subscribe(OnTimerSecondsChanged));

            ApplySafeArea();
            ApplyBoardWidth();

            return UniTask.CompletedTask;
        }


#if UNITY_EDITOR
        private void Update()
        {
            if (false == Application.isPlaying || null == ViewModel)
                return;

            ApplySafeArea();
            ApplyBoardWidth();
        }
#endif


        private void OnTimerSecondsChanged(int totalSeconds)
        {
            if (false == _timerTMP)
                return;

            var minutes = totalSeconds / 60;
            var seconds = totalSeconds % 60;
            _timerTMP.text = $"{minutes:00}:{seconds:00}";
        }

        private void ApplySafeArea()
        {
            if (false == _topContainer || false == _canvas)
                return;

            var topInsetPx = Screen.height - Screen.safeArea.yMax;
            var topInsetCanvas = topInsetPx / _canvas.scaleFactor;
            _topContainer.anchoredPosition = new Vector2(0f, -topInsetCanvas);
        }

        private void ApplyBoardWidth()
        {
            if (false == _topContainer || false == _canvas)
                return;

            var cam = Camera.main;
            if (false == cam)
                return;

            var isOverlay = _canvas.renderMode == RenderMode.ScreenSpaceOverlay;
            var overlayCamera = isOverlay ? null : _canvas.worldCamera;
            var referenceRect = _topContainer.parent as RectTransform;
            if (false == referenceRect)
                return;

            var leftWorld = new Vector3(ViewModel.BoardCenterX - ViewModel.BoardHalfWidth, 0f, 0f);
            var rightWorld = new Vector3(ViewModel.BoardCenterX + ViewModel.BoardHalfWidth, 0f, 0f);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                referenceRect, cam.WorldToScreenPoint(leftWorld), overlayCamera, out var leftLocal);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                referenceRect, cam.WorldToScreenPoint(rightWorld), overlayCamera, out var rightLocal);

            var boardCanvasWidth = rightLocal.x - leftLocal.x;
            _topContainer.sizeDelta = new Vector2(boardCanvasWidth, _topContainer.sizeDelta.y);
        }
    }
}