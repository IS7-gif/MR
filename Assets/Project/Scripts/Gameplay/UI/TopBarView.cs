using Cysharp.Threading.Tasks;
using Project.Scripts.Gameplay.Battle.HUD;
using Project.Scripts.Services.UISystem;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.UI
{
    public class TopBarView : BaseView<BattleHUDViewModel>
    {
        [Tooltip("Корневой RectTransform верхнего контейнера - получает смещение по Y safe area во время выполнения")]
        [SerializeField] private RectTransform _topContainer;

        [Tooltip("Отображает имя противника")]
        [SerializeField] private TMP_Text _enemyNameText;

        [Tooltip("Зарезервировано для таймера или дополнительного ярлыка - привязка добавится при готовности сервиса таймера")]
        [SerializeField] private TMP_Text _secondaryText;


        private Canvas _canvas;


        protected override UniTask OnBindViewModel()
        {
            _canvas = GetComponentInParent<Canvas>();

            if (_enemyNameText)
                _enemyNameText.text = ViewModel.EnemyName;

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