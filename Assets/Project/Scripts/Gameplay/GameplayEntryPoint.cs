using System;
using Cysharp.Threading.Tasks;
using Project.Scripts.Configs;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Configs.Board;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Configs.UI;
using Project.Scripts.Gameplay.Battle.Board;
using Project.Scripts.Gameplay.Battle.HUD;
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
        [SerializeField] private Transform _tileContainer;

        [Tooltip("Компонент View, задающий размер рамки доски и маски спавна во время выполнения")]
        [SerializeField] private BoardView _boardView;

        private EventBus _eventBus;
        private AudioService _audioService;
        private BoardConfig _boardConfig;
        private LevelConfig _levelConfig;
        private BoardAnimationConfig _animConfig;
        private InputConfig _inputConfig;
        private CascadeEnergyConfig _cascadeEnergyConfig;
        private SpecialTileConfig _specialTileConfig;
        private UIConfig _uiConfig;
        private BattleViewConfig _battleViewConfig;
        private UIService _uiService;
        private MoveBarViewModel _moveBarViewModel;
        private IGameStateService _gameStateService;
        private IBoardRuntimeService _boardRuntimeService;
        private IMoveBarService _moveBarService;
        private GameResultPresenter _gameResultPresenter;
        private GameResultSequenceController _gameResultSequenceController;
        private BattleHUDViewModel _battleHUDViewModel;
        private IBoardBoundsProvider _boardBoundsProvider;
        private BattleHUDView _battleHUDView;
        private InputService _inputService;
        private SwapInputHandler _swapHandler;
        private BoardOrchestrator _orchestrator;
        private GameAudioController _gameAudioController;
        private IBattleTimerService _battleTimerService;
        private IBattleFlowService _battleFlowService;
        private IOvertimeService _overtimeService;
        private IUnitActivationCooldownService _unitActivationCooldownService;
        private HintConfig _hintConfig;
        private TileKindPaletteConfig _palette;
        private HintService _hintService;
        private DebugConfig _debugConfig;
        private IDisposable _boardRuntimeStateSubscription;

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

            _battleTimerService?.Tick(Time.deltaTime);
            _overtimeService?.Tick(Time.deltaTime);

            if (false == _gameStateService.IsPlaying)
                return;

            _battleFlowService?.Tick(Time.deltaTime);
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
            if (_moveBarService?.IsEnabled == true)
                _uiService?.Close<MoveBarView>();

            _uiService?.Close<TopBarView>();

            if (_battleHUDView)
                _battleHUDView.Close();

            _hintService?.Dispose();
            _orchestrator?.Dispose();
            _swapHandler?.Dispose();
            _inputService?.Dispose();
            _boardRuntimeStateSubscription?.Dispose();
            _boardRuntimeStateSubscription = null;

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
            LevelConfig levelConfig,
            BoardAnimationConfig animConfig,
            InputConfig inputConfig,
            CascadeEnergyConfig cascadeEnergyConfig,
            SpecialTileConfig specialTileConfig,
            UIConfig uiConfig,
            BattleViewConfig battleViewConfig,
            UIService uiService,
            MoveBarViewModel moveBarViewModel,
            IGameStateService gameStateService,
            IBoardRuntimeService boardRuntimeService,
            IMoveBarService moveBarService,
            GameResultPresenter gameResultPresenter,
            GameResultSequenceController gameResultSequenceController,
            BattleHUDViewModel battleHUDViewModel,
            IBoardBoundsProvider boardBoundsProvider,
            IBattleTimerService battleTimerService,
            IBattleFlowService battleFlowService,
            IOvertimeService overtimeService,
            IUnitActivationCooldownService unitActivationCooldownService,
            HintConfig hintConfig,
            TileKindPaletteConfig palette,
            DebugConfig debugConfig)
        {
            _eventBus = eventBus;
            _audioService = audioService;
            _boardConfig = boardConfig;
            _levelConfig = levelConfig;
            _animConfig = animConfig;
            _inputConfig = inputConfig;
            _cascadeEnergyConfig = cascadeEnergyConfig;
            _specialTileConfig = specialTileConfig;
            _uiConfig = uiConfig;
            _battleViewConfig = battleViewConfig;
            _uiService = uiService;
            _moveBarViewModel = moveBarViewModel;
            _gameStateService = gameStateService;
            _boardRuntimeService = boardRuntimeService;
            _moveBarService = moveBarService;
            _gameResultPresenter = gameResultPresenter;
            _gameResultSequenceController = gameResultSequenceController;
            _battleHUDViewModel = battleHUDViewModel;
            _boardBoundsProvider = boardBoundsProvider;
            _battleTimerService = battleTimerService;
            _battleFlowService = battleFlowService;
            _overtimeService = overtimeService;
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
            var (frameWidth, frameHeight, _) = ComputeFrameDimensions();
            var boardCenter = ComputeBoardCenter();
            _boardView.transform.position = boardCenter;

            var boardTopWorldY = boardCenter.y + frameHeight * 0.5f;
            var boardHalfWidth = frameWidth * 0.5f;
            _boardBoundsProvider.SetBounds(boardCenter.x, boardTopWorldY, boardHalfWidth, cellSize);

            await _uiService.Show<TopBarView, BattleHUDViewModel>(_battleHUDViewModel);

            _inputService = new InputService(_inputConfig);

            var hudGo = Instantiate(_battleViewConfig.BattleHUDViewPrefab);
            hudGo.name = _battleViewConfig.BattleHUDViewPrefab.name;
            _battleHUDView = hudGo.GetComponent<BattleHUDView>();
            _battleHUDView.SetDependencies(_inputService, _battleViewConfig, _boardBoundsProvider);
            await _battleHUDView.InitializeAsync(_battleHUDViewModel);
            await _battleHUDView.ShowAsync();
            _gameResultSequenceController.BindVisuals(_battleHUDView);

            var pool = new TilePool(_boardConfig.TilePrefab, _tileContainer, _animConfig, cellSize, _boardConfig.TileScale);
            var matchFinder = new MatchFinder(_boardConfig.MinMatchLength);
            var gridManager = new GridManager(_levelConfig, _animConfig, pool, cellSize, _boardRuntimeService);
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

            _boardView.Setup(frameWidth, frameHeight, cellSize, _boardConfig.MaskTopPadding);

            var gravityHandler = new GravityHandler(gridManager.State, gridManager, pool, _levelConfig, _boardRuntimeService);

            _swapHandler = new SwapInputHandler(_inputService, gridManager.State, gridManager, _inputConfig.WorldDragThreshold, _inputConfig.ReanchorOnUnlock);

            var moveChecker = new MoveChecker(gridManager.State, gridManager, matchFinder, _levelConfig);
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
                _levelConfig, _gameStateService, _boardRuntimeService, _eventBus, _palette);

            _gameAudioController = new GameAudioController(_audioService, _eventBus, _gameStateService);
            _gameAudioController.StartMusic();

            _boardRuntimeStateSubscription?.Dispose();
            _boardRuntimeStateSubscription = _boardRuntimeService.State.Subscribe(_ => RefreshPhaseOverlays());
            RefreshPhaseOverlays();

            _gameResultPresenter.Initialize();
            _gameResultSequenceController.Initialize();
            _battleTimerService.Initialize();
            _battleFlowService.Initialize();

#if UNITY_EDITOR
            var editHandler = gameObject.AddComponent<BoardEditClickHandler>();
            editHandler.Init(gridManager.State, gridManager, _levelConfig, cellSize);
#endif

            await _inputService.InitAsync();
            await _swapHandler.InitAsync();
            await _orchestrator.InitAsync();
            await _orchestrator.StartGame();
        }

#if UNITY_EDITOR
        private void OnLayoutChanged()  => ApplyLiveLayout();
        private void OnBattleLayoutChanged() => _battleHUDView?.RefreshPosition();

        private void ApplyLiveResize() => ApplyLiveLayout();

        private void ApplyLiveLayout()
        {
            _cellSize = ComputeTileCellSize();
            var (frameWidth, frameHeight, _) = ComputeFrameDimensions();
            var boardCenter = ComputeBoardCenter();
            _boardView.transform.position = boardCenter;

            _gridManager.SetCellSize(_cellSize);

            var newOrigin = ComputeGridOrigin(boardCenter, _cellSize);
            _gridManager.SetOrigin(newOrigin);
            _gridManager.RepositionAllTiles();

            _boardView.Setup(frameWidth, frameHeight, _cellSize, _boardConfig.MaskTopPadding);
            _pool.UpdateScale(_cellSize, _boardConfig.TileScale);

            var boardTopWorldY = boardCenter.y + frameHeight * 0.5f;
            var boardHalfWidth = frameWidth * 0.5f;
            _boardBoundsProvider.SetBounds(boardCenter.x, boardTopWorldY, boardHalfWidth, _cellSize);

            _battleHUDView?.RefreshPosition();
        }
#endif

        private (float width, float height, float cellSize) ComputeFrameDimensions()
        {
            var cam  = Camera.main;
            var camHeight = cam.orthographicSize * 2f;
            var camWidth = camHeight * cam.aspect;
            var effectiveWidth = Mathf.Min(camWidth, camHeight * _boardConfig.MaxAspectRatio);

            var byWidth = effectiveWidth * (1f - _boardConfig.FramePaddingPercent) / _levelConfig.Width;
            var byHeight = camHeight * (1f - _boardConfig.UIReservedHeightPercent) / _levelConfig.Height;
            var cellSize = Mathf.Min(byWidth, byHeight);

            var frameWidth  = _levelConfig.Width  * cellSize;
            var frameHeight = _levelConfig.Height * cellSize + _boardConfig.FrameExtraHeight;
            return (frameWidth, frameHeight, cellSize);
        }

        private float ComputeTileCellSize()
        {
            var cam = Camera.main;
            var camHeight = cam.orthographicSize * 2f;
            var camWidth = camHeight * cam.aspect;
            var effectiveWidth = Mathf.Min(camWidth, camHeight * _boardConfig.MaxAspectRatio);

            var byWidth = effectiveWidth * (1f - _boardConfig.TilePaddingPercent) / _levelConfig.Width;
            var byHeight = camHeight * (1f - _boardConfig.UIReservedHeightPercent) / _levelConfig.Height;

            return Mathf.Min(byWidth, byHeight);
        }

        private Vector3 ComputeBoardCenter()
        {
            var cam = Camera.main;
            var (_, frameHeight, frameCellSize) = ComputeFrameDimensions();
            var camBottomY = cam.transform.position.y - cam.orthographicSize;
            var bottomPadding = _boardConfig.BoardBottomPadding * frameCellSize;

            return new Vector3(
                cam.transform.position.x,
                camBottomY + bottomPadding + frameHeight * 0.5f,
                0f
            );
        }

        private Vector3 ComputeGridOrigin(Vector3 boardCenter, float cellSize)
        {
            return boardCenter + new Vector3(
                -(_levelConfig.Width - 1) * cellSize * 0.5f,
                -(_levelConfig.Height - 1) * cellSize * 0.5f,
                0f
            );
        }

        private void RefreshPhaseOverlays()
        {
            _boardView?.SetInteractionOverlayActive(!_boardRuntimeService.CanAcceptInput);
        }
    }
}