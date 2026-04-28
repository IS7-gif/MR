using Cysharp.Threading.Tasks;
using Project.Scripts.Configs.Board;
using Project.Scripts.Configs.UI;
using Project.Scripts.Gameplay.Results;
using Project.Scripts.Gameplay.Battle.Targeting;
using Project.Scripts.Gameplay.Battle.Units;
using Project.Scripts.Gameplay.UI;
using Project.Scripts.Services.Input;
using Project.Scripts.Services.UISystem;
using Project.Scripts.Shared.Heroes;
using R3;
using UnityEngine;
using UnityEngine.Pool;

namespace Project.Scripts.Gameplay.Battle.HUD
{
    public class BattleFieldView : BaseView<BattleFieldViewModel>, IGameResultVisuals
    {
        private const int FloatingNumberDefaultPoolCapacity = 4;
        private const int FloatingNumberMaxPoolSize = 16;
        
        
        [Tooltip("Четыре вида слота героя для стороны игрока, упорядочены слева направо (индексы 0-3)")]
        [SerializeField] private HeroSlotView[] _playerHeroSlots;

        [Tooltip("Четыре вида слота героя для стороны врага, упорядочены слева направо (индексы 0-3)")]
        [SerializeField] private HeroSlotView[] _enemyHeroSlots;

        [Tooltip("Вид слота аватара игрока")]
        [SerializeField] private AvatarSlotView _playerAvatarSlot;

        [Tooltip("Вид слота аватара врага")]
        [SerializeField] private AvatarSlotView _enemyAvatarSlot;

        [Tooltip("Обрабатывает жесты перетаскивания к цели в мировом пространстве")]
        [SerializeField] private TargetingInputHandler _targetingInputHandler;

        [Tooltip("Префаб с компонентом FloatingDamageNumber")]
        [SerializeField] private FloatingDamageNumber _floatingDamagePrefab;

        [Tooltip("Optional HUD-side energy orb FX view")]
        [SerializeField] private BattleEnergyFXView _energyFXView;

        [Tooltip("Опциональный SpriteRenderer затемнения боевого поля; включается, когда боевые действия недоступны")]
        [SerializeField] private SpriteRenderer _phaseOverlay;

        [Header("Layout Geometry")]
        [Tooltip("Высота визуальной области боевого поля в world units. Управляет Background, Floor, layout anchors и позициями panel rows.")]
        [Min(0.01f)]
        [SerializeField] private float _layoutHeight = 4.2f;

        [Tooltip("Sliced SpriteRenderer фоновой рамки боевого поля.")]
        [SerializeField] private SpriteRenderer _backgroundRenderer;

        [Tooltip("Внутренняя подложка поля, масштабируется по Y пропорционально Layout Height.")]
        [SerializeField] private Transform _floorTransform;

        [Tooltip("Панель героев игрока, позиционируется от нижней границы Layout Height.")]
        [SerializeField] private Transform _playerPanel;

        [Tooltip("Панель героев врага, позиционируется от верхней границы Layout Height.")]
        [SerializeField] private Transform _enemyPanel;

        [Tooltip("Floor.localScale.y на одну единицу Layout Height.")]
        [SerializeField] private float _floorScaleYPerLayoutHeight = 89.50952f;

        [Tooltip("Отступ PlayerPanel от нижней границы Layout Height.")]
        [SerializeField] private float _playerPanelBottomPadding = 1.11f;

        [Tooltip("Отступ EnemyPanel от верхней границы Layout Height.")]
        [SerializeField] private float _enemyPanelTopPadding = 1.09f;

        [Tooltip("Маркер нижнего края визуальной области боевого поля; используется для вертикального автостекинга блоков над доской матчинга")]
        [SerializeField] private Transform _layoutBottomAnchor;

        [SerializeField] private Transform _layoutTopAnchor;

        [Space(10)]
        
        [Tooltip("Щит первой группы героев врага - скрывается, когда первая группа уничтожена")]
        [SerializeField] private GroupShieldView _enemyGroup1Shield;

        [Tooltip("Щит второй группы героев врага - скрывается, когда вторая группа уничтожена")]
        [SerializeField] private GroupShieldView _enemyGroup2Shield;
        
        [Tooltip("Щит первой группы героев игрока - скрывается, когда первая группа уничтожена")]
        [SerializeField] private GroupShieldView _playerGroup1Shield;

        [Tooltip("Щит второй группы героев игрока - скрывается, когда вторая группа уничтожена")]
        [SerializeField] private GroupShieldView _playerGroup2Shield;


        private IInputService _inputService;
        private ObjectPool<FloatingDamageNumber> _floatingPool;
        private TileKindPaletteConfig _tileKindPalette;
        private Transform _playerEnergyAbsorbTarget;


        protected override UniTask OnBindViewModel()
        {
            ApplyBattleFieldGeometry();
            BindSlots();
            BindGroupShields();
            SetupTargeting();
            BindShieldPulse();
            SetupEnergyFX();
            SetupFloatingNumbers();
            BindInteractionOverlay();

            return UniTask.CompletedTask;
        }

        protected override void OnClose()
        {
            CleanupRuntimeResources();
        }

        public void ReleaseSceneInstance()
        {
            CleanupRuntimeResources();
        }


        public void SetDependencies(IInputService inputService, TileKindPaletteConfig tileKindPalette, Transform playerEnergyAbsorbTarget)
        {
            _inputService = inputService;
            _tileKindPalette = tileKindPalette;
            _playerEnergyAbsorbTarget = playerEnergyAbsorbTarget;
        }

        public async UniTask PlayAvatarPulse(BattleSide side, AvatarPulseStepConfig config)
        {
            var targetView = side == BattleSide.Player
                ? _playerAvatarSlot
                : _enemyAvatarSlot;

            if (false == targetView)
                return;

            await targetView.PlayResultPulse(config);
        }

        public void RefreshPosition()
        {
            ApplyBattleFieldGeometry();
        }

        public void SetLayoutBottomWorldY(float worldY)
        {
            ApplyBattleFieldGeometry();
            float pivotToBottom;

            if (_layoutBottomAnchor)
            {
                pivotToBottom = transform.position.y - _layoutBottomAnchor.position.y;
            }
            else
            {
                var renderers = GetComponentsInChildren<SpriteRenderer>(false);
                var minY = transform.position.y;
                for (var i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i].sprite)
                        minY = Mathf.Min(minY, renderers[i].bounds.min.y);
                }
                pivotToBottom = transform.position.y - minY;
            }

            var pos = transform.position;
            transform.position = new Vector3(pos.x, worldY + pivotToBottom, pos.z);
        }

        public float GetLayoutHeight()
        {
            ApplyBattleFieldGeometry();

            if (_layoutHeight > 0f)
                return _layoutHeight;

            var renderers = GetComponentsInChildren<SpriteRenderer>(false);
            if (renderers.Length == 0)
                return 0f;

            var minY = float.PositiveInfinity;
            var maxY = float.NegativeInfinity;
            for (var i = 0; i < renderers.Length; i++)
            {
                if (false == renderers[i].sprite)
                    continue;

                minY = Mathf.Min(minY, renderers[i].bounds.min.y);
                maxY = Mathf.Max(maxY, renderers[i].bounds.max.y);
            }

            return float.IsInfinity(minY) || float.IsInfinity(maxY) ? 0f : maxY - minY;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ApplyBattleFieldGeometry();
        }
#endif

        private void ApplyBattleFieldGeometry()
        {
            var safeHeight = Mathf.Max(0.01f, _layoutHeight);
            if (false == Mathf.Approximately(_layoutHeight, safeHeight))
                _layoutHeight = safeHeight;

            if (_backgroundRenderer)
            {
                var size = _backgroundRenderer.size;
                if (false == Mathf.Approximately(size.y, safeHeight))
                    _backgroundRenderer.size = new Vector2(size.x, safeHeight);
            }

            if (_floorTransform)
            {
                var scale = _floorTransform.localScale;
                var floorScaleY = safeHeight * _floorScaleYPerLayoutHeight;
                if (false == Mathf.Approximately(scale.y, floorScaleY))
                    _floorTransform.localScale = new Vector3(scale.x, floorScaleY, scale.z);
            }

            if (_phaseOverlay)
            {
                var overlayTransform = _phaseOverlay.transform;
                var overlayScale = overlayTransform.localScale;
                var overlayScaleX = CalculateRendererScaleX(_phaseOverlay, _backgroundRenderer);
                var overlayScaleY = CalculateRendererScaleY(_phaseOverlay, safeHeight);

                if (false == Mathf.Approximately(overlayScale.x, overlayScaleX)
                    || false == Mathf.Approximately(overlayScale.y, overlayScaleY))
                {
                    overlayTransform.localScale = new Vector3(overlayScaleX, overlayScaleY, overlayScale.z);
                }
            }

            var halfHeight = safeHeight * 0.5f;

            if (_layoutBottomAnchor)
                SetLocalY(_layoutBottomAnchor, -halfHeight);

            if (_layoutTopAnchor)
                SetLocalY(_layoutTopAnchor, halfHeight);

            if (_playerPanel)
                SetLocalY(_playerPanel, -halfHeight + _playerPanelBottomPadding);

            if (_enemyPanel)
                SetLocalY(_enemyPanel, halfHeight - _enemyPanelTopPadding);
        }

        private static void SetLocalY(Transform target, float y)
        {
            var localPosition = target.localPosition;
            if (Mathf.Approximately(localPosition.y, y))
                return;

            target.localPosition = new Vector3(localPosition.x, y, localPosition.z);
        }

        private static float CalculateRendererScaleX(SpriteRenderer targetRenderer, SpriteRenderer sourceRenderer)
        {
            if (false == targetRenderer || false == targetRenderer.sprite)
                return 1f;

            var width = sourceRenderer ? sourceRenderer.size.x : targetRenderer.size.x;
            var spriteWidth = targetRenderer.sprite.bounds.size.x;
            return spriteWidth > 0f ? width / spriteWidth : 1f;
        }

        private static float CalculateRendererScaleY(SpriteRenderer targetRenderer, float height)
        {
            if (false == targetRenderer || false == targetRenderer.sprite)
                return 1f;

            var spriteHeight = targetRenderer.sprite.bounds.size.y;
            return spriteHeight > 0f ? height / spriteHeight : 1f;
        }

        private void BindSlots()
        {
            _playerAvatarSlot?.Bind(ViewModel.PlayerAvatar, ViewModel.GroupDefense, ViewModel.DeathConfig);
            _enemyAvatarSlot?.Bind(ViewModel.EnemyAvatar, ViewModel.GroupDefense, ViewModel.DeathConfig);

            BindHeroRow(_playerHeroSlots, ViewModel.PlayerHeroSlots);
            BindHeroRow(_enemyHeroSlots, ViewModel.EnemyHeroSlots);
        }

        private void BindGroupShields()
        {
            var playerDef = ViewModel.GroupDefense.PlayerDefense;
            var enemyDef = ViewModel.GroupDefense.EnemyDefense;

            _playerGroup1Shield?.Bind(playerDef.Select(s => false == s.IsGroup1Destroyed));
            _playerGroup2Shield?.Bind(playerDef.Select(s => false == s.IsGroup2Destroyed));
            _enemyGroup1Shield?.Bind(enemyDef.Select(s => false == s.IsGroup1Destroyed));
            _enemyGroup2Shield?.Bind(enemyDef.Select(s => false == s.IsGroup2Destroyed));
        }

        private void BindShieldPulse()
        {
            if (false == _targetingInputHandler || false == ViewModel.BattleAnimConfig)
                return;

            var config = ViewModel.BattleAnimConfig.ShieldPulse;

            _targetingInputHandler.IsHoveringBlockedAvatar
                .Subscribe(hovering =>
                {
                    if (hovering)
                    {
                        _enemyGroup1Shield?.StartPulse(config);
                        _enemyGroup2Shield?.StartPulse(config);
                    }
                    else
                    {
                        _enemyGroup1Shield?.StopPulse();
                        _enemyGroup2Shield?.StopPulse();
                    }
                })
                .AddTo(Disposables);
        }

        private void BindHeroRow(HeroSlotView[] views, HeroSlotViewModel[] viewModels)
        {
            if (null == views || null == viewModels)
                return;

            var count = Mathf.Min(views.Length, viewModels.Length);
            for (var i = 0; i < count; i++)
            {
                if (views[i])
                    views[i].Bind(viewModels[i], ViewModel.BattleAnimConfig, ViewModel.DeathConfig);
            }
        }

        private void SetupTargeting()
        {
            if (false == _targetingInputHandler || null == _inputService)
                return;

            var registry = new TargetingRegistry();

            if (_playerAvatarSlot)
                registry.Register(_playerAvatarSlot);

            if (_enemyAvatarSlot)
                registry.Register(_enemyAvatarSlot);

            if (_playerHeroSlots != null)
                for (var i = 0; i < _playerHeroSlots.Length; i++)
                    if (_playerHeroSlots[i])
                        registry.Register(_playerHeroSlots[i]);

            if (_enemyHeroSlots != null)
                for (var i = 0; i < _enemyHeroSlots.Length; i++)
                    if (_enemyHeroSlots[i])
                        registry.Register(_enemyHeroSlots[i]);

            _targetingInputHandler.Init(
                _inputService,
                registry,
                ViewModel.AbilityExecution,
                ViewModel.GameStateService,
                ViewModel.BattleActionRuntime,
                Camera.main);
        }

        private void SetupEnergyFX()
        {
            if (false == _energyFXView)
                return;

            _energyFXView.Initialize(
                ViewModel.EventBus,
                _tileKindPalette,
                _playerEnergyAbsorbTarget,
                ViewModel.BattleAnimConfig);
        }

        private void BindInteractionOverlay()
        {
            if (false == _phaseOverlay)
                return;

            _phaseOverlay.enabled = ViewModel.IsInteractionOverlayVisible.CurrentValue;
            ViewModel.IsInteractionOverlayVisible
                .Subscribe(active => _phaseOverlay.enabled = active)
                .AddTo(Disposables);
        }

        private void SetupFloatingNumbers()
        {
            if (false == _floatingDamagePrefab)
                return;

            _floatingPool = new ObjectPool<FloatingDamageNumber>(
                createFunc: () => Instantiate(_floatingDamagePrefab, transform),
                actionOnGet: c => c.gameObject.SetActive(true),
                actionOnRelease: c => { c.Kill(); c.gameObject.SetActive(false); },
                actionOnDestroy: c => { if (c) Destroy(c.gameObject); },
                defaultCapacity: FloatingNumberDefaultPoolCapacity,
                maxSize: FloatingNumberMaxPoolSize);

            ViewModel.EnemyAvatar.Hit
                .Subscribe(dmg => SpawnFloatingNumber(dmg, FloatingNumberType.Damage, _enemyAvatarSlot.HitAnchor))
                .AddTo(Disposables);

            ViewModel.PlayerAvatar.Hit
                .Subscribe(dmg => SpawnFloatingNumber(dmg, FloatingNumberType.Damage, _playerAvatarSlot.HitAnchor))
                .AddTo(Disposables);

            ViewModel.EnemyAvatar.Heal
                .Subscribe(amt => SpawnFloatingNumber(amt, FloatingNumberType.Heal, _enemyAvatarSlot.HitAnchor))
                .AddTo(Disposables);

            ViewModel.PlayerAvatar.Heal
                .Subscribe(amt => SpawnFloatingNumber(amt, FloatingNumberType.Heal, _playerAvatarSlot.HitAnchor))
                .AddTo(Disposables);

            BindHeroFloatingNumbers(_playerHeroSlots, ViewModel.PlayerHeroSlots);
            BindHeroFloatingNumbers(_enemyHeroSlots, ViewModel.EnemyHeroSlots);
        }

        private void BindHeroFloatingNumbers(HeroSlotView[] views, HeroSlotViewModel[] viewModels)
        {
            if (null == views || null == viewModels)
                return;

            var count = Mathf.Min(views.Length, viewModels.Length);
            for (var i = 0; i < count; i++)
            {
                if (false == views[i] || false == viewModels[i].IsAssigned)
                    continue;

                var anchor = views[i].HitAnchor;
                var vm = viewModels[i];

                vm.Hit
                    .Subscribe(dmg => SpawnFloatingNumber(dmg, FloatingNumberType.Damage, anchor))
                    .AddTo(Disposables);

                vm.Heal
                    .Subscribe(amt => SpawnFloatingNumber(amt, FloatingNumberType.Heal, anchor))
                    .AddTo(Disposables);
            }
        }

        private void SpawnFloatingNumber(int value, FloatingNumberType type, Transform anchor)
        {
            if (null == _floatingPool || false == anchor)
                return;

            var item = _floatingPool.Get();
            item.Play(value, type, anchor, ViewModel.BattleAnimConfig,
                () => _floatingPool.Release(item));
        }

        private void CleanupRuntimeResources()
        {
            _energyFXView?.Cleanup();
            _floatingPool?.Dispose();
            _floatingPool = null;
        }
    }
}