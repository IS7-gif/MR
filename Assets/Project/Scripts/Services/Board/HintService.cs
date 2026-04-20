using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Scripts.Configs.Board;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Game;
using Project.Scripts.Services.Grid;
using Project.Scripts.Shared;
using Project.Scripts.Shared.Grid;
using Project.Scripts.Shared.Tiles;
using R3;
using UnityEngine;

namespace Project.Scripts.Services.Board
{
    public class HintService : IHintService, IDisposable
    {
        private readonly HintConfig _config;
        private readonly IGridState _gridState;
        private readonly IGridView _gridView;
        private readonly IMatchFinder _matchFinder;
        private readonly LevelConfig _levelConfig;
        private readonly IGameStateService _gameStateService;
        private readonly IBoardRuntimeService _boardRuntimeService;
        private readonly EventBus _eventBus;
        private readonly TileKindPaletteConfig _palette;
        private IDisposable _matchSub;
        private IDisposable _stateSub;
        private CancellationTokenSource _cts;
        private Tiles.Tile _firstHintedTile;
        private Tiles.Tile _secondHintedTile;


        public HintService(
            HintConfig config,
            IGridState gridState,
            IGridView gridView,
            IMatchFinder matchFinder,
            LevelConfig levelConfig,
            IGameStateService gameStateService,
            IBoardRuntimeService boardRuntimeService,
            EventBus eventBus,
            TileKindPaletteConfig palette)
        {
            _config = config;
            _gridState = gridState;
            _gridView = gridView;
            _matchFinder = matchFinder;
            _levelConfig = levelConfig;
            _gameStateService = gameStateService;
            _boardRuntimeService = boardRuntimeService;
            _eventBus = eventBus;
            _palette = palette;

            _matchSub = _eventBus.Subscribe<MatchPlayedEvent>(OnMatchPlayed);
            _stateSub = _gameStateService.State.Subscribe(OnGameStateChanged);

            RestartTimer();
        }

        public void Dispose()
        {
            HideHint();
            _matchSub?.Dispose();
            _matchSub = null;
            _stateSub?.Dispose();
            _stateSub = null;
            CancelTimer();
        }


        private void OnMatchPlayed(MatchPlayedEvent _)
        {
            HideHint();
            RestartTimer();
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state != GameState.Playing && state != GameState.Overtime)
            {
                HideHint();
                CancelTimer();
            }
            else if (!_firstHintedTile && !_secondHintedTile) 
                RestartTimer();
        }

        private void RestartTimer()
        {
            CancelTimer();
            _cts = new CancellationTokenSource();
            RunTimerAsync(_cts.Token).Forget();
        }

        private void CancelTimer()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private async UniTaskVoid RunTimerAsync(CancellationToken ct)
        {
            await UniTask.Delay(
                TimeSpan.FromSeconds(_config.IdleTimeThreshold),
                cancellationToken: ct);

            if (ct.IsCancellationRequested)
                return;

            if (false == _gameStateService.IsPlaying)
                return;

            if (false == _boardRuntimeService.CanAcceptInput)
                return;

            var candidate = HintSelector.Select(_gridState, _gridView, _matchFinder, _levelConfig);
            if (false == candidate.IsValid)
                return;

            ShowHint(candidate);
        }

        private void ShowHint(HintCandidate candidate)
        {
            var firstTile = _gridView.GetTile(candidate.FirstTile);
            var secondTile = _gridView.GetTile(candidate.SecondTile);
            if (!firstTile || !secondTile)
                return;

            _firstHintedTile = firstTile;
            _secondHintedTile = secondTile;

            if (_config.ShowGlow)
            {
                ShowHintGlow(firstTile);
                ShowHintGlow(secondTile);
            }

            if (_config.AnimatePulse)
            {
                firstTile.Animator.AnimateHintPulse(_config);
                secondTile.Animator.AnimateHintPulse(_config);
            }
        }

        private void HideHint()
        {
            if (!_firstHintedTile && !_secondHintedTile)
                return;

            if (_config.ShowGlow)
            {
                HideHintGlow(_firstHintedTile);
                HideHintGlow(_secondHintedTile);
            }

            if (_config.AnimatePulse)
            {
                StopHintPulse(_firstHintedTile);
                StopHintPulse(_secondHintedTile);
            }

            _firstHintedTile = null;
            _secondHintedTile = null;
        }

        private void ShowHintGlow(Tiles.Tile tile)
        {
            var color = tile.Kind.IsColor() ? _palette.GetColor(tile.Kind, Color.white) : Color.white;
            tile.SetGlowActive(true, color);
        }

        private static void HideHintGlow(Tiles.Tile tile)
        {
            if (!tile)
                return;

            tile.SetGlowActive(false, Color.white);
        }

        private static void StopHintPulse(Tiles.Tile tile)
        {
            if (!tile)
                return;

            tile.Animator.StopHintPulse();
        }
    }
}