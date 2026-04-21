using Cysharp.Threading.Tasks;
using Project.Scripts.Configs.UI;
using Project.Scripts.Gameplay.Results;
using Project.Scripts.Gameplay.Battle.Targeting;
using Project.Scripts.Gameplay.Battle.Units;
using Project.Scripts.Gameplay.UI;
using Project.Scripts.Services.Board;
using Project.Scripts.Services.Input;
using Project.Scripts.Services.UISystem;
using Project.Scripts.Shared.Heroes;
using R3;
using UnityEngine;
using UnityEngine.Pool;

namespace Project.Scripts.Gameplay.Battle.HUD
{
    public class BattleHUDView : BaseView<BattleHUDViewModel>, IGameResultVisuals
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

        [Tooltip("Трансформ, задающий базовую позицию для объявлений на доске; Vertical World Offset из BoardAnnouncementConfig применяется относительно него")]
        [SerializeField] private Transform _announcementAnchor;


        private IInputService _inputService;
        private IBoardBoundsProvider _boardBounds;
        private ObjectPool<FloatingDamageNumber> _floatingPool;


        protected override UniTask OnBindViewModel()
        {
            BindSlots();
            PublishAnnouncementAnchor();
            SetupTargeting();
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


        public void SetDependencies(IInputService inputService, IBoardBoundsProvider boardBounds)
        {
            _inputService = inputService;
            _boardBounds = boardBounds;
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

#if UNITY_EDITOR
        public void RefreshPosition()
        {
            PublishAnnouncementAnchor();
        }
#endif

        private void PublishAnnouncementAnchor()
        {
            if (_boardBounds == null || false == _announcementAnchor)
                return;

            _boardBounds.SetAnnouncementAnchorY(_announcementAnchor.position.y);
        }

        private void BindSlots()
        {
            _playerAvatarSlot?.Bind(ViewModel.PlayerAvatar, ViewModel.GroupDefense, ViewModel.DeathConfig);
            _enemyAvatarSlot?.Bind(ViewModel.EnemyAvatar, ViewModel.GroupDefense, ViewModel.DeathConfig);

            BindHeroRow(_playerHeroSlots, ViewModel.PlayerHeroSlots);
            BindHeroRow(_enemyHeroSlots, ViewModel.EnemyHeroSlots);
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
                _playerHeroSlots,
                ViewModel.PlayerHeroSlots,
                ViewModel.PlayerHeroKinds,
                _playerAvatarSlot,
                ViewModel.PlayerAvatar.SlotColor,
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