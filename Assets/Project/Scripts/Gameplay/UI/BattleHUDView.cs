using Cysharp.Threading.Tasks;
using Project.Scripts.Services.UISystem;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Gameplay.UI
{
    public class BattleHUDView : BaseView<BattleHUDViewModel>
    {
        [Header("Enemy")]
        [Tooltip("RectTransform of the enemy avatar panel — anchor to top of screen")]
        [SerializeField] private RectTransform _enemyPanel;

        [Tooltip("Image displaying the enemy avatar portrait")]
        [SerializeField] private Image _enemyAvatar;

        [Tooltip("Fill image for the enemy HP bar (Type=Filled, Method=Horizontal)")]
        [SerializeField] private Image _enemyHPBar;

        [Header("Player")]
        [Tooltip("RectTransform of the player avatar panel — positioned at board top edge at runtime")]
        [SerializeField] private RectTransform _playerPanel;

        [Tooltip("Image displaying the player avatar portrait")]
        [SerializeField] private Image _playerAvatar;

        [Tooltip("Fill image for the player HP bar (Type=Filled, Method=Horizontal)")]
        [SerializeField] private Image _playerHPBar;

        [Header("Layout")]
        [Tooltip("Extra vertical offset in canvas units added above the board top edge (positive = higher)")]
        [SerializeField] private float _playerPanelPadding = 50f;

#if UNITY_EDITOR
        private RectTransform _cachedReferenceRect;
#endif


        protected override UniTask OnBindViewModel()
        {
            if (_enemyAvatar)
                _enemyAvatar.sprite = ViewModel.EnemyAvatarSprite;
            if (_playerAvatar)
                _playerAvatar.sprite = ViewModel.PlayerAvatarSprite;

            _enemyHPBar.color = ViewModel.EnemyHPBarColor;
            _playerHPBar.color = ViewModel.PlayerHPBarColor;

            _enemyHPBar.fillAmount = ViewModel.EnemyHPFill.CurrentValue;
            _playerHPBar.fillAmount = ViewModel.PlayerHPFill.CurrentValue;

            ViewModel.EnemyHPFill
                .Skip(1)
                .Subscribe(v => _enemyHPBar.fillAmount = v)
                .AddTo(Disposables);

            ViewModel.PlayerHPFill
                .Skip(1)
                .Subscribe(v => _playerHPBar.fillAmount = v)
                .AddTo(Disposables);

            var referenceRect = _playerPanel.parent as RectTransform;
#if UNITY_EDITOR
            _cachedReferenceRect = referenceRect;
#endif
            PositionPlayerPanel(ViewModel.BoardTopWorldY, referenceRect);

            return UniTask.CompletedTask;
        }


#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying || ViewModel == null || !_cachedReferenceRect)
                return;

            PositionPlayerPanel(ViewModel.BoardTopWorldY, _cachedReferenceRect);
        }
#endif

        private void PositionPlayerPanel(float boardTopWorldY, RectTransform referenceRect)
        {
            var cam = Camera.main;
            if (!cam)
                return;

            var canvas = GetComponentInParent<Canvas>();
            if (!canvas || !referenceRect)
                return;

            var worldPos = new Vector3(cam.transform.position.x, boardTopWorldY, 0f);
            var screenPoint = (Vector2)cam.WorldToScreenPoint(worldPos);

            var isOverlay = canvas.renderMode == RenderMode.ScreenSpaceOverlay;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                referenceRect,
                screenPoint,
                isOverlay ? null : canvas.worldCamera,
                out var localPoint);

            var referenceHeight = referenceRect.rect.height;
            _playerPanel.anchoredPosition = new Vector2(0f, localPoint.y + referenceHeight * 0.5f + _playerPanelPadding);
        }
    }
}