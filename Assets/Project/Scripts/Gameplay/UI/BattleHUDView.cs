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
        [Tooltip("RectTransform of the enemy avatar panel — anchored to top of screen")]
        [SerializeField] private RectTransform _enemyPanel;

        [Tooltip("Image displaying the enemy avatar portrait")]
        [SerializeField] private Image _enemyAvatar;

        [Tooltip("Fill image for the enemy HP bar (Type=Filled, Method=Horizontal)")]
        [SerializeField] private Image _enemyHPBar;

        [Header("Enemy Heroes")]
        [Tooltip("Four HeroSlotView components for the enemy side, ordered left to right")]
        [SerializeField] private HeroSlotView[] _enemyHeroSlots;

        [Header("Player")]
        [Tooltip("RectTransform of the player avatar panel — positioned at board top edge at runtime")]
        [SerializeField] private RectTransform _playerPanel;

        [Tooltip("Image displaying the player avatar portrait")]
        [SerializeField] private Image _playerAvatar;

        [Tooltip("Fill image for the player HP bar (Type=Filled, Method=Horizontal)")]
        [SerializeField] private Image _playerHPBar;

        [Header("Player Heroes")]
        [Tooltip("Four HeroSlotView components for the player side, ordered left to right")]
        [SerializeField] private HeroSlotView[] _playerHeroSlots;

        [Header("Layout")]
        [Tooltip("Extra vertical offset in canvas units added above the board top edge (positive = higher)")]
        [SerializeField] private float _playerPanelPadding = 50f;

        private Vector2 _enemyPanelBasePosition;

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

            BindHeroSlots(_enemyHeroSlots, ViewModel.EnemyHeroSlots);
            BindHeroSlots(_playerHeroSlots, ViewModel.PlayerHeroSlots);

            _enemyPanelBasePosition = _enemyPanel.anchoredPosition;
            ApplyEnemySafeAreaOffset();

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

            ApplyEnemySafeAreaOffset();
            PositionPlayerPanel(ViewModel.BoardTopWorldY, _cachedReferenceRect);
        }
#endif

        private void ApplyEnemySafeAreaOffset()
        {
            var canvas = GetComponentInParent<Canvas>();
            if (!canvas) 
                return;

            var topInsetPx = Screen.height - Screen.safeArea.yMax;
            var topInsetCanvas = topInsetPx / canvas.scaleFactor;

            _enemyPanel.anchoredPosition = new Vector2(
                _enemyPanelBasePosition.x,
                _enemyPanelBasePosition.y - topInsetCanvas);
        }

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

        private void BindHeroSlots(HeroSlotView[] views, HeroSlotViewModel[] viewModels)
        {
            if (null == views || null == viewModels)
                return;

            var count = Mathf.Min(views.Length, viewModels.Length);
            for (var i = 0; i < count; i++)
            {
                if (views[i])
                    views[i].Bind(viewModels[i]);
            }
        }
    }
}