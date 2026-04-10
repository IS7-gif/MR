using System;
using Cysharp.Threading.Tasks;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Configs.UI;
using Project.Scripts.Gameplay.UI;
using Project.Scripts.Services.Announcements;
using Project.Scripts.Services.Combat;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Game;
using Project.Scripts.Services.Progression;
using Project.Scripts.Services.UISystem;
using R3;

namespace Project.Scripts.Gameplay
{
    public class GameResultPresenter : IDisposable
    {
        private readonly IGameStateService _gameStateService;
        private readonly UIService _uiService;
        private readonly UIConfig _uiConfig;
        private readonly IMoveCounterService _moveCounter;
        private readonly ILevelProgressionService _progression;
        private readonly BattleAnimationConfig _battleAnimConfig;
        private readonly LevelConfig _levelConfig;
        private readonly IBoardAnnouncementService _announcementService;


        private readonly EventBus _eventBus;
        private IDisposable _stateSub;
        private IDisposable _resultSub;
        private bool _lastResultIsFlawless;


        public GameResultPresenter(
            IGameStateService gameStateService,
            UIService uiService,
            UIConfig uiConfig,
            IMoveCounterService moveCounter,
            ILevelProgressionService progression,
            BattleAnimationConfig battleAnimConfig,
            LevelConfig levelConfig,
            EventBus eventBus,
            IBoardAnnouncementService announcementService)
        {
            _gameStateService = gameStateService;
            _uiService = uiService;
            _uiConfig = uiConfig;
            _moveCounter = moveCounter;
            _progression = progression;
            _battleAnimConfig = battleAnimConfig;
            _levelConfig = levelConfig;
            _eventBus = eventBus;
            _announcementService = announcementService;
        }


        public void Initialize()
        {
            _uiService.RegisterView<WinView>(_uiConfig.WinViewPrefab, UILayer.Popup);
            _uiService.RegisterView<LoseView>(_uiConfig.LoseViewPrefab, UILayer.Popup);

            _resultSub = _eventBus.Subscribe<GameResultEvent>(OnGameResult);
            _stateSub = _gameStateService.State.Subscribe(OnStateChanged);
        }

        public void Dispose()
        {
            _resultSub?.Dispose();
            _resultSub = null;
            _stateSub?.Dispose();
            _stateSub = null;
        }


        private void OnStateChanged(GameState state)
        {
            if (state == GameState.Win)
                ShowWin().Forget();
            else if (state == GameState.Lose)
                ShowLose().Forget();
        }

        private void OnGameResult(GameResultEvent e)
        {
            _lastResultIsFlawless = e.IsFlawless;
        }

        private async UniTaskVoid ShowWin()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_battleAnimConfig.ResultScreenDelay));

            if (_lastResultIsFlawless)
                await _announcementService.Show("Flawless Victory!");

            var bot = _levelConfig.BotConfig;
            var viewModel = new WinViewModel(_moveCounter, _progression,
                _levelConfig.LevelId,
                bot ? bot.OpponentName : string.Empty,
                _lastResultIsFlawless,
                () => _uiService.Close<WinView>());
            await _uiService.Show<WinView, WinViewModel>(viewModel);
        }

        private async UniTaskVoid ShowLose()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_battleAnimConfig.ResultScreenDelay));
            var bot = _levelConfig.BotConfig;
            var viewModel = new LoseViewModel(_moveCounter, _progression,
                _levelConfig.LevelId,
                bot ? bot.OpponentName : string.Empty,
                () => _uiService.Close<LoseView>());
            await _uiService.Show<LoseView, LoseViewModel>(viewModel);
        }
    }
}