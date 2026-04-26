using System;
using Cysharp.Threading.Tasks;
using Project.Scripts.Configs;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Configs.Board;
using Project.Scripts.Configs.Grid;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Configs.UI;
using Project.Scripts.Gameplay.Battle.HUD;
using Project.Scripts.Gameplay.Battle.Layout;
using Project.Scripts.Gameplay.Results;
using Project.Scripts.Gameplay.UI;
using Project.Scripts.Services.Audio;
using Project.Scripts.Services.Board;
using Project.Scripts.Services.BattleFlow;
using Project.Scripts.Services.Combat;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Game;
using Project.Scripts.Shared.Rules;
using Project.Scripts.Services.Audio.AudioSystem;
using Project.Scripts.Services.Grid;
using Project.Scripts.Services.Input;
using Project.Scripts.Services.Timer;
using Project.Scripts.Services.UISystem;
using Project.Scripts.Shared.BattleFlow;
using Project.Scripts.Shared.Grid;
using UnityEngine;
using VContainer;
using R3;
#if UNITY_EDITOR
using Project.Scripts.Services.BoardEdit;
#endif

namespace Project.Scripts.Gameplay
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        [Tooltip("Родительский Transform для всех инстанцируемых объектов тайлов")]
        [SerializeField] private BattleWorldLayout _battleWorldLayout;

        private EventBus _eventBus;
        private AudioService _audioService;
        private BoardConfig _boardConfig;
        private GridConfig _gridConfig;
        private LevelConfig _levelConfig;
        private BoardAnimationConfig _animConfig;
        private InputConfig _inputConfig;
        private CascadeEnergyConfig _cascadeEnergyConfig;
        private SpecialTileConfig _specialTileConfig;
        private UIConfig _uiConfig;
        private BattleViewConfig _battleViewConfig;
        private BattleFlowConfig _battleFlowConfig;
        private UIService _uiService;
        private MoveBarViewModel _moveBarViewModel;
        private IGameStateService _gameStateService;
        private IBoardRuntimeService _boardRuntimeService;
        private IMoveBarService _moveBarService;
        private GameResultPresenter _gameResultPresenter;
        private GameResultSequenceController _gameResultSequenceController;
        private BattleFieldViewModel _battleFieldViewModel;
        private IBoardBoundsProvider _boardBoundsProvider;
        private BattleFieldView _battleFieldView;
        private InputService _inputService;
        private SwapInputHandler _swapHandler;
        private BoardOrchestrator _orchestrator;
        private GameAudioController _gameAudioController;
        private IBattleFlowService _battleFlowService;
        private IBurndownService _burndownService;
        private BurndownConfig _burndownConfig;
        private IUnitActivationCooldownService _unitActivationCooldownService;
        private HintConfig _hintConfig;
        private TileKindPaletteConfig _palette;
        private HintService _hintService;
        private DebugConfig _debugConfig;
        private IDisposable _boardRuntimeSubscription;
        private IDisposable _gameStateSubscription;
        private IDisposable _battleFlowPhaseSubscription;
        private IDisposable _burndownStartedSubscription;

#if UNITY_EDITOR
        private GridManager _gridManager;
        private TilePool _pool;
        private float _cellSize;
        private int _lastWidth;
        private int _lastHeight;
#endif

        private void Start()
        {
            InitAsync().Forget();
        }

        private void Update()
        {
            if (null == _moveBarService)
                return;

            if (false == _gameStateService.IsPlaying)
                return;

            var isPrePhase = _battleFlowService is { IsInitialized: true, IsPrePhase: true };
            _battleFlowService?.Tick(Time.deltaTime);

            if (isPrePhase)
                return;

            _burndownService?.Tick(Time.deltaTime);
            _unitActivationCooldownService?.Tick(Time.deltaTime);

            if (_moveBarService.IsEnabled && _boardRuntimeService.CanAcceptInput)
                _moveBarService.Tick(Time.deltaTime);

#if UNITY_EDITOR
            if (_gridManager == null)
                return;

            if (Screen.width == _lastWidth && Screen.height == _lastHeight)
                return;

            _lastWidth = Screen.width;
            _lastHeight = Screen.height;
            ApplyLiveResize();
#endif
        }

        private void OnDestroy()
        {
            _battleFieldView?.ReleaseSceneInstance();
            _battleWorldLayout?.EnergyView?.Cleanup();

            if (_moveBarService?.IsEnabled == true)
                _uiService?.Close<MoveBarView>();

            _uiService?.Close<TopBarView>();

            _hintService?.Dispose();
            _orchestrator?.Dispose();
            _swapHandler?.Dispose();
            _inputService?.Dispose();
            _boardRuntimeSubscription?.Dispose();
            _boardRuntimeSubscription = null;
            _gameStateSubscription?.Dispose();
            _gameStateSubscription = null;
            _battleFlowPhaseSubscription?.Dispose();
            _battleFlowPhaseSubscription = null;
            _burndownStartedSubscription?.Dispose();
            _burndownStartedSubscription = null;

#if UNITY_EDITOR
            BoardConfig.LayoutChanged -= OnLayoutChanged;
            BattleViewConfig.LayoutChanged -= OnBattleLayoutChanged;
#endif
        }

        [Inject]
        public void Construct(
            EventBus eventBus,
            AudioService audioService,
            BoardConfig boardConfig,
            GridConfig gridConfig,
            LevelConfig levelConfig,
            BoardAnimationConfig animConfig,
            InputConfig inputConfig,
            CascadeEnergyConfig cascadeEnergyConfig,
            SpecialTileConfig specialTileConfig,
            UIConfig uiConfig,
            BattleViewConfig battleViewConfig,
            BattleFlowConfig battleFlowConfig,
            UIService uiService,
            MoveBarViewModel moveBarViewModel,
            IGameStateService gameStateService,
            IBoardRuntimeService boardRuntimeService,
            IMoveBarService moveBarService,
            GameResultPresenter gameResultPresenter,
            GameResultSequenceController gameResultSequenceController,
            BattleFieldViewModel battleHUDViewModel,
            IBoardBoundsProvider boardBoundsProvider,
            IBattleFlowService battleFlowService,
            IBurndownService burndownService,
            BurndownConfig burndownConfig,
            IUnitActivationCooldownService unitActivationCooldownService,
            HintConfig hintConfig,
            TileKindPaletteConfig palette,
            DebugConfig debugConfig)
        {
            _eventBus = eventBus;
            _audioService = audioService;
            _boardConfig = boardConfig;
            _gridConfig = gridConfig;
            _levelConfig = levelConfig;
            _animConfig = animConfig;
            _inputConfig = inputConfig;
            _cascadeEnergyConfig = cascadeEnergyConfig;
            _specialTileConfig = specialTileConfig;
            _uiConfig = uiConfig;
            _battleViewConfig = battleViewConfig;
            _battleFlowConfig = battleFlowConfig;
            _uiService = uiService;
            _moveBarViewModel = moveBarViewModel;
            _gameStateService = gameStateService;
            _boardRuntimeService = boardRuntimeService;
            _moveBarService = moveBarService;
            _gameResultPresenter = gameResultPresenter;
            _gameResultSequenceController = gameResultSequenceController;
            _battleFieldViewModel = battleHUDViewModel;
            _boardBoundsProvider = boardBoundsProvider;
            _battleFlowService = battleFlowService;
            _burndownService = burndownService;
            _burndownConfig = burndownConfig;
            _unitActivationCooldownService = unitActivationCooldownService;
            _hintConfig = hintConfig;
            _palette = palette;
            _debugConfig = debugConfig;
        }

        private async UniTaskVoid InitAsync()
        {
            _moveBarService.Initialize();

            if (_moveBarService.IsEnabled)
                _uiService.RegisterView<MoveBarView>(_uiConfig.MoveBarViewPrefab, UILayer.MainDynamic);

            _uiService.RegisterView<TopBarView>(_uiConfig.TopBarViewPrefab, UILayer.Main);

            if (_moveBarService.IsEnabled)
                await _uiService.Show<MoveBarView, MoveBarViewModel>(_moveBarViewModel);

            var cellSize = ComputeTileCellSize();
            var (frameWidth, frameHeight, frameCellSize) = ComputeFrameDimensions();
            var boardCenter = ComputeBoardCenter(frameHeight, frameCellSize);
            _battleWorldLayout.SetBoardWorldCenter(boardCenter);

            var boardTopWorldY = boardCenter.y + frameHeight * 0.5f;
            var boardHalfWidth = frameWidth * 0.5f;
            _boardBoundsProvider.SetBounds(boardCenter.x, boardTopWorldY, boardHalfWidth, cellSize);

            await _uiService.Show<TopBarView, BattleFieldViewModel>(_battleFieldViewModel);

            _inputService = new InputService(_inputConfig);

            _battleFieldView = _battleWorldLayout.BattleFieldView;
            _battleFieldView.SetDependencies(
                _inputService,
                _boardBoundsProvider,
                _palette,
                _battleWorldLayout.EnergyView ? _battleWorldLayout.EnergyView.PlayerEnergyAbsorbTarget : null);
            await _battleFieldView.InitializeAsync(_battleFieldViewModel);
            await _battleFieldView.ShowAsync();
            _battleWorldLayout.EnergyView?.Bind(_battleFieldViewModel);

            _battleWorldLayout.SetVerticalLayout(
                boardTopWorldY,
                cellSize,
                _battleViewConfig.GapBoardToPlayerEnergy,
                _battleViewConfig.GapPlayerEnergyToEnemyEnergy,
                _battleViewConfig.GapEnemyEnergyToBattleField);
            _battleWorldLayout.RefreshBindings();

            _gameResultSequenceController.BindVisuals(_battleFieldView);

            var pool = new TilePool(_boardConfig.TilePrefab, _battleWorldLayout.TileContainer, _animConfig, cellSize, _boardConfig.TileScale);
            var matchFinder = new MatchFinder(MatchRules.MinMatchLength);
            var gridManager = new GridManager(_levelConfig, _gridConfig, _animConfig, pool, cellSize, _boardRuntimeService);
            gridManager.SetOrigin(ComputeGridOrigin(boardCenter, cellSize));

#if UNITY_EDITOR
            _pool = pool;
            _gridManager = gridManager;
            _cellSize = cellSize;
            _lastWidth = Screen.width;
            _lastHeight = Screen.height;
            BoardConfig.LayoutChanged += OnLayoutChanged;
            BattleViewConfig.LayoutChanged += OnBattleLayoutChanged;
#endif

            _battleWorldLayout.BoardView.Setup(frameWidth, frameHeight, cellSize, _boardConfig.MaskTopPadding);

            var gravityHandler = new GravityHandler(gridManager.State, gridManager, pool, _gridConfig, _boardRuntimeService);

            _swapHandler = new SwapInputHandler(_inputService, gridManager.State, gridManager, _inputConfig.WorldDragThreshold, _inputConfig.ReanchorOnUnlock);

            var moveChecker = new MoveChecker(gridManager.State, gridManager, matchFinder, _gridConfig);
            var specialTileResolver = new SpecialTileResolver(_specialTileConfig, _levelConfig);
            var swapComboResolver = new SwapComboResolver();

            _orchestrator = new BoardOrchestrator(
                _eventBus,
                gridManager.State,
                gridManager,
                gridManager,
                gravityHandler,
                matchFinder,
                _swapHandler,
                moveChecker,
                _cascadeEnergyConfig,
                _gameStateService,
                _boardRuntimeService,
                _moveBarService,
                specialTileResolver,
                swapComboResolver,
                _debugConfig);

            _hintService = new HintService(_hintConfig, gridManager.State, gridManager, matchFinder,
                _gridConfig, _gameStateService, _boardRuntimeService, _eventBus, _palette);

            _gameAudioController = new GameAudioController(_audioService, _eventBus, _gameStateService);
            _gameAudioController.StartMusic();

            _boardRuntimeSubscription?.Dispose();
            _boardRuntimeSubscription = _boardRuntimeService.State.Subscribe(_ => RefreshPhaseOverlays());
            _gameStateSubscription?.Dispose();
            _gameStateSubscription = _gameStateService.State.Subscribe(_ => RefreshPhaseOverlays());
            _battleFlowPhaseSubscription?.Dispose();
            _battleFlowPhaseSubscription = _eventBus.Subscribe<BattleFlowPhaseChangedEvent>(_ => RefreshPhaseOverlays());
            RefreshPhaseOverlays();

            _burndownStartedSubscription?.Dispose();
            _burndownStartedSubscription = _eventBus.Subscribe<BurndownStartedEvent>(_ =>
            {
                gridManager.CollapseAll(_burndownConfig.CollapseAllDuration, _burndownConfig.CollapseAllEase).Forget();
            });

            _gameResultPresenter.Initialize();
            _gameResultSequenceController.Initialize();
            _battleFlowService.Initialize();

#if UNITY_EDITOR
            var editHandler = gameObject.AddComponent<BoardEditClickHandler>();
            editHandler.Init(gridManager.State, gridManager, _gridConfig, cellSize);
#endif

            await _inputService.InitAsync();
            await _swapHandler.InitAsync();
            await _orchestrator.InitAsync();
            await _orchestrator.StartGame();
        }

#if UNITY_EDITOR
        private void OnLayoutChanged()  => ApplyLiveLayout();
        private void OnBattleLayoutChanged() => ApplyLiveLayout();

        private void ApplyLiveResize() => ApplyLiveLayout();

        private void ApplyLiveLayout()
        {
            _cellSize = ComputeTileCellSize();
            var (frameWidth, frameHeight, frameCellSize) = ComputeFrameDimensions();
            var boardCenter = ComputeBoardCenter(frameHeight, frameCellSize);
            _battleWorldLayout.SetBoardWorldCenter(boardCenter);

            _gridManager.SetCellSize(_cellSize);

            var newOrigin = ComputeGridOrigin(boardCenter, _cellSize);
            _gridManager.SetOrigin(newOrigin);
            _gridManager.RepositionAllTiles();

            _battleWorldLayout.BoardView.Setup(frameWidth, frameHeight, _cellSize, _boardConfig.MaskTopPadding);
            _pool.UpdateScale(_cellSize, _boardConfig.TileScale);

            var boardTopWorldY = boardCenter.y + frameHeight * 0.5f;
            var boardHalfWidth = frameWidth * 0.5f;
            _boardBoundsProvider.SetBounds(boardCenter.x, boardTopWorldY, boardHalfWidth, _cellSize);

            _battleWorldLayout?.SetVerticalLayout(
                boardTopWorldY,
                _cellSize,
                _battleViewConfig.GapBoardToPlayerEnergy,
                _battleViewConfig.GapPlayerEnergyToEnemyEnergy,
                _battleViewConfig.GapEnemyEnergyToBattleField);
            _battleWorldLayout?.RefreshBindings();
        }
#endif

        private (float width, float height, float cellSize) ComputeFrameDimensions()
        {
            var cam  = Camera.main;
            var camHeight = cam.orthographicSize * 2f;
            var camWidth = camHeight * cam.aspect;
            var effectiveWidth = Mathf.Min(camWidth, camHeight * _boardConfig.MaxAspectRatio);

            var byWidth = effectiveWidth * (1f - _boardConfig.FramePaddingPercent) / _gridConfig.Width;
            var byHeight = camHeight * (1f - _boardConfig.UIReservedHeightPercent) / _gridConfig.Height;
            var cellSize = Mathf.Min(byWidth, byHeight);

            var frameWidth  = _gridConfig.Width  * cellSize;
            var frameHeight = _gridConfig.Height * cellSize + _boardConfig.FrameExtraHeight;
            return (frameWidth, frameHeight, cellSize);
        }

        private float ComputeTileCellSize()
        {
            var cam = Camera.main;
            var camHeight = cam.orthographicSize * 2f;
            var camWidth = camHeight * cam.aspect;
            var effectiveWidth = Mathf.Min(camWidth, camHeight * _boardConfig.MaxAspectRatio);

            var byWidth = effectiveWidth * (1f - _boardConfig.TilePaddingPercent) / _gridConfig.Width;
            var byHeight = camHeight * (1f - _boardConfig.UIReservedHeightPercent) / _gridConfig.Height;

            return Mathf.Min(byWidth, byHeight);
        }

        private Vector3 ComputeBoardCenter(float frameHeight, float frameCellSize)
        {
            var cam = Camera.main;
            var camBottomY = cam.transform.position.y - cam.orthographicSize;
            var bottomPadding = _battleViewConfig.BattleWorldBottomPadding * frameCellSize;

            return new Vector3(
                cam.transform.position.x,
                camBottomY + bottomPadding + frameHeight * 0.5f,
                0f
            );
        }

        private Vector3 ComputeGridOrigin(Vector3 boardCenter, float cellSize)
        {
            return boardCenter + new Vector3(
                -(_gridConfig.Width - 1) * cellSize * 0.5f,
                -(_gridConfig.Height - 1) * cellSize * 0.5f,
                0f
            );
        }

        private void RefreshPhaseOverlays()
        {
            var showOverlay = ShouldShowBoardOverlay();
            _battleWorldLayout?.BoardView?.SetInteractionOverlayActive(showOverlay);
        }

        private bool ShouldShowBoardOverlay()
        {
            if (_gameStateService.State.CurrentValue != GameState.Playing)
                return false;

            if (_battleFlowService is { IsInitialized: true, IsPrePhase: true })
            {
                if (_battleFlowConfig.DimCurrentPhaseDuringPrePhase)
                    return true;

                return _battleFlowService.Snapshot.UpcomingPhase == BattlePhaseKind.Hero;
            }

            return false == _boardRuntimeService.CanAcceptInput;
        }
    }
}