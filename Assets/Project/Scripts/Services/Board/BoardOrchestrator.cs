using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Project.Scripts.Configs;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Tiles.Behaviours;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Game;
using Project.Scripts.Services.Grid;
using Project.Scripts.Services.Combat;
using Project.Scripts.Services.Input;
using Project.Scripts.Shared;
using Project.Scripts.Shared.Grid;
using Project.Scripts.Shared.Input;
using Project.Scripts.Shared.Rules;
using Project.Scripts.Shared.Tiles;
using Project.Scripts.Tiles;
using UnityEngine;

namespace Project.Scripts.Services.Board
{
    public class BoardOrchestrator : IBoardOrchestrator, IDisposable
    {
        private const int ShuffleMaxAttempts = 10;


        private readonly IGridState _state;
        private readonly IGridView _view;
        private readonly IGridOperations _gridOps;
        private readonly IGravityHandler _gravity;
        private readonly IMatchFinder _matchFinder;
        private readonly ISwapInputHandler _swapHandler;
        private readonly IMoveChecker _moveChecker;
        private readonly CascadeEnergyConfig _cascadeEnergyConfig;
        private readonly IGameStateService _gameStateService;
        private readonly IMoveBarService _moveBarService;
        private readonly EventBus _eventBus;
        private readonly SpecialTileResolver _specialTileResolver;
        private readonly SwapComboResolver _swapComboResolver;
        private readonly DebugConfig _debugConfig;
        private bool _isProcessing;


        public BoardOrchestrator(EventBus eventBus, IGridState state, IGridView view, IGridOperations gridOps,
            IGravityHandler gravity, IMatchFinder matchFinder, ISwapInputHandler swapHandler,
            IMoveChecker moveChecker, CascadeEnergyConfig cascadeEnergyConfig,
            IGameStateService gameStateService, IMoveBarService moveBarService,
            SpecialTileResolver specialTileResolver, SwapComboResolver swapComboResolver,
            DebugConfig debugConfig)
        {
            _eventBus = eventBus;
            _state = state;
            _view = view;
            _gridOps = gridOps;
            _gravity = gravity;
            _matchFinder = matchFinder;
            _swapHandler = swapHandler;
            _moveChecker = moveChecker;
            _cascadeEnergyConfig = cascadeEnergyConfig;
            _gameStateService = gameStateService;
            _moveBarService = moveBarService;
            _specialTileResolver = specialTileResolver;
            _swapComboResolver = swapComboResolver;
            _debugConfig = debugConfig;
        }

        public UniTask InitAsync()
        {
            _swapHandler.OnSwapRequested += OnSwapRequested;
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            _swapHandler.OnSwapRequested -= OnSwapRequested;
        }

        public async UniTask StartGame()
        {
            await _gridOps.PopulateGrid();
        }


        private void OnSwapRequested(SwapRequest request)
        {
            if (_isProcessing)
                return;

            if (false == _gameStateService.IsPlaying)
                return;

            if (false == _moveBarService.HasMoves)
            {
                _eventBus.Publish(new SwapRejectedEvent());
                _swapHandler.NotifyBoardReady();
                return;
            }

            HandleSwapAsync(request).Forget();
        }

        private async UniTask HandleSwapAsync(SwapRequest request)
        {
            var fromTile = _view.GetTile(request.From);
            var toTile = _view.GetTile(request.To);

            if (false == fromTile || false == toTile)
            {
                _swapHandler.NotifyBoardReady();
                return;
            }

            _isProcessing = true;
            try
            {
                var fromKind = fromTile.Config.Kind;
                var toKind = toTile.Config.Kind;
                var fromIsSpecial = fromKind.IsSpecial();
                var toIsSpecial = toKind.IsSpecial();

                await _gridOps.SwapTiles(request.From, request.To);

                var waves = new List<List<MatchResult>>();
                var energyByKind = new Dictionary<TileKind, float>();
                var moveUsed = false;

                if (fromIsSpecial && toIsSpecial)
                {
                    _moveBarService.TryConsume();

                    var stateBefore = _state.GetGridState();
                    await ExecuteSwapCombo(fromKind, toKind, request.To, request.From, fromTile, toTile);
                    var stateAfter = _state.GetGridState();

                    _eventBus.Publish(new BombActivatedEvent());
                    AccumulateGridDiffEnergy(stateBefore, stateAfter, energyByKind);

                    await RunPostActivationFlow(waves, request.PivotPosition);
                    moveUsed = true;
                }
                else if (fromIsSpecial || toIsSpecial)
                {
                    _moveBarService.TryConsume();

                    Tile specialTile, partnerTile;
                    GridPoint specialFinalPos;

                    if (fromIsSpecial)
                    {
                        specialTile = fromTile;
                        specialFinalPos = request.To;
                        partnerTile = toTile;
                    }
                    else
                    {
                        specialTile = toTile;
                        specialFinalPos = request.From;
                        partnerTile = fromTile;
                    }

                    var stateBefore = _state.GetGridState();
                    await ActivateSpecialWithPartner(specialTile, partnerTile, specialFinalPos);
                    var stateAfter = _state.GetGridState();

                    _eventBus.Publish(new BombActivatedEvent());
                    AccumulateGridDiffEnergy(stateBefore, stateAfter, energyByKind);

                    await RunPostActivationFlow(waves, request.PivotPosition);
                    moveUsed = true;
                }
                else
                {
                    var matches = _matchFinder.FindMatches(_state.GetGridState());

                    if (matches.Count == 0)
                        await _gridOps.SwapTiles(request.To, request.From);
                    else
                    {
                        _moveBarService.TryConsume();
                        await ProcessMatchChain(matches, waves, request.PivotPosition, true);
                        moveUsed = true;
                    }
                }

                AccumulateMatchEnergy(waves, _cascadeEnergyConfig, energyByKind);

                if (energyByKind.Count > 0)
                {
                    _eventBus.Publish(new EnergyGeneratedEvent(energyByKind));
                    if (_debugConfig.LogCascades)
                        Debug.Log(BuildDetailedCascadeLog(waves, _cascadeEnergyConfig, energyByKind));
                }

                if (moveUsed)
                    _eventBus.Publish(new MoveUsedEvent());
            }
            finally
            {
                _isProcessing = false;
                _swapHandler.NotifyBoardReady();
            }
        }

        private async UniTask ActivateSpecialWithPartner(Tile specialTile, Tile partnerTile, GridPoint specialFinalPos)
        {
            if (specialTile.Config.Kind == TileKind.Storm)
                specialTile.SetPayloadKind(partnerTile.Kind);

            await _gridOps.ActivateBySwap(specialFinalPos);
        }

        private async UniTask ExecuteSwapCombo(TileKind kindA, TileKind kindB,
            GridPoint posA, GridPoint posB, Tile tileA, Tile tileB)
        {
            var comboType = _swapComboResolver.Resolve(kindA, kindB);

            switch (comboType)
            {
                case SwapComboType.StormStorm:
                    await ExecuteStormStormCombo(posA, posB);
                    break;

                case SwapComboType.StormBomb:
                {
                    var stormPos = kindA == TileKind.Storm ? posA : posB;
                    await ExecuteStormBombCombo(stormPos);
                    break;
                }

                case SwapComboType.StormLine:
                {
                    var stormPos = kindA == TileKind.Storm ? posA : posB;
                    await ExecuteStormLineCombo(stormPos);
                    break;
                }

                case SwapComboType.BombBomb:
                {
                    var doubleRadius = GetBombDoubleRadius(tileA, tileB);
                    await ExecuteBombBombCombo(posA, posB, doubleRadius);
                    break;
                }

                case SwapComboType.BombLine:
                {
                    var bombPos = kindA == TileKind.Bomb ? posA : posB;
                    var bombTile = kindA == TileKind.Bomb ? tileA : tileB;
                    var radius = GetBombRadius(bombTile);
                    await ExecuteBombLineCombo(posA, posB, bombPos, radius);
                    break;
                }

                case SwapComboType.LineLine:
                    await ExecuteLineLineCombo(posA, posB);
                    break;
            }
        }

        private async UniTask ExecuteStormStormCombo(GridPoint posA, GridPoint posB)
        {
            await UniTask.WhenAll(_gridOps.ConsumeTile(posA), _gridOps.ConsumeTile(posB));
            var allPositions = _state.GetAllOccupied();
            await _gridOps.ActivateTiles(allPositions);
        }

        private async UniTask ExecuteStormBombCombo(GridPoint stormPos)
        {
            await _gridOps.ConsumeTile(stormPos);
            var bombPositions = _state.GetAllOfKind(TileKind.Bomb);
            if (bombPositions.Count == 0)
                return;

            await _gridOps.ActivateTiles(bombPositions);
        }

        private async UniTask ExecuteStormLineCombo(GridPoint stormPos)
        {
            await _gridOps.ConsumeTile(stormPos);
            var linePositions = _state.GetAllOfKind(TileKind.LineRuneH);
            linePositions.AddRange(_state.GetAllOfKind(TileKind.LineRuneV));
            if (linePositions.Count == 0)
                return;

            await _gridOps.ActivateTiles(linePositions);
        }

        private async UniTask ExecuteBombBombCombo(GridPoint posA, GridPoint posB, int doubleRadius)
        {
            await UniTask.WhenAll(_gridOps.ConsumeTile(posA), _gridOps.ConsumeTile(posB));

            var explosion = new HashSet<GridPoint>(_state.GetNeighboursInRadius(posA, doubleRadius));
            var ps = _state.GetNeighboursInRadius(posB, doubleRadius);
            for (var i = 0; i < ps.Count; i++)
                explosion.Add(ps[i]);

            await _gridOps.ActivateTiles(new List<GridPoint>(explosion));
        }

        private async UniTask ExecuteBombLineCombo(GridPoint posA, GridPoint posB,
            GridPoint bombPos, int bombRadius)
        {
            await UniTask.WhenAll(_gridOps.ConsumeTile(posA), _gridOps.ConsumeTile(posB));

            var area = new HashSet<GridPoint>(_state.GetNeighboursInRadius(bombPos, bombRadius));
            var row = _state.GetAllInRow(bombPos.Y);
            for (var i = 0; i < row.Count; i++)
                area.Add(row[i]);

            var ps = _state.GetAllInColumn(bombPos.X);
            for (var i = 0; i < ps.Count; i++)
                area.Add(ps[i]);

            await _gridOps.ActivateTiles(new List<GridPoint>(area));
        }

        private async UniTask ExecuteLineLineCombo(GridPoint posA, GridPoint posB)
        {
            await UniTask.WhenAll(_gridOps.ConsumeTile(posA), _gridOps.ConsumeTile(posB));

            var cross = new HashSet<GridPoint>(_state.GetAllInRow(posA.Y));
            var col = _state.GetAllInColumn(posA.X);
            for (var i = 0; i < col.Count; i++)
                cross.Add(col[i]);

            await _gridOps.ActivateTiles(new List<GridPoint>(cross));
        }

        private async UniTask RunPostActivationFlow(List<List<MatchResult>> waves, GridPoint pivotPosition)
        {
            await _gravity.ApplyGravity();
            await _gravity.SpawnNewTiles();
            var chainMatches = _matchFinder.FindMatches(_state.GetGridState());
            if (chainMatches.Count > 0)
                await ProcessMatchChain(chainMatches, waves, pivotPosition, false);

            await EnsureMovesAvailable();
        }

        private async UniTask ProcessMatchChain(List<MatchResult> matches, List<List<MatchResult>> waves, GridPoint pivotPosition, bool spawnSpecials)
        {
            while (matches.Count > 0)
            {
                var cascadeLevel = waves.Count + 1;
                _eventBus.Publish(new MatchPlayedEvent(cascadeLevel));
                waves.Add(new List<MatchResult>(matches));

                var specialPlacements = spawnSpecials && cascadeLevel == 1
                    ? _specialTileResolver.Resolve(matches, pivotPosition) : null;
                await _gridOps.RemoveMatches(matches, specialPlacements);
                await _gravity.ApplyGravity();
                await _gravity.SpawnNewTiles();
                matches = _matchFinder.FindMatches(_state.GetGridState());
            }

            await EnsureMovesAvailable();
        }

        private async UniTask EnsureMovesAvailable()
        {
            if (_moveChecker.HasPossibleMoves())
                return;

            for (var i = 0; i < ShuffleMaxAttempts; i++)
            {
                await _gridOps.ShuffleGrid();
                if (_moveChecker.HasPossibleMoves())
                    return;
            }

            _gridOps.ForceInjectMove();

            var immediateMatches = _matchFinder.FindMatches(_state.GetGridState());
            if (immediateMatches.Count > 0)
                await ProcessMatchChain(immediateMatches, new List<List<MatchResult>>(), GridPoint.Zero, false);
        }

        private static int GetBombRadius(Tile tile)
        {
            return (tile.Config.Behaviour as BombTileBehaviour)?.Radius ?? 1;
        }

        private static int GetBombDoubleRadius(Tile tileA, Tile tileB)
        {
            var bomb = tileA.Config.Behaviour as BombTileBehaviour
                    ?? tileB.Config.Behaviour as BombTileBehaviour;

            return bomb?.DoubleRadius ?? 2;
        }

        private static void AccumulateMatchEnergy(List<List<MatchResult>> waves, CascadeEnergyConfig config, Dictionary<TileKind, float> energy)
        {
            for (var i = 0; i < waves.Count; i++)
            {
                var matches = waves[i];
                var cascadeMult = 1f + config.CascadeMultiplierStep * i;
                var multiMatchMult = matches.Count > 1 ? config.MultiMatchMultiplier : 1f;

                for (var j = 0; j < matches.Count; j++)
                {
                    var match = matches[j];
                    if (false == match.TileKind.IsColor())
                        continue;

                    var shapeMult = match.Shape switch
                    {
                        MatchShape.LShape => config.LShapeMultiplier,
                        MatchShape.TShape => config.TShapeMultiplier,
                        _ => 1f
                    };

                    energy.TryGetValue(match.TileKind, out var current);
                    energy[match.TileKind] = current + match.Positions.Count * shapeMult * cascadeMult * multiMatchMult;
                }
            }
        }

        private static string BuildDetailedCascadeLog(List<List<MatchResult>> waves, CascadeEnergyConfig config, Dictionary<TileKind, float> energyByKind)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("[Energy] === Cascade report ===");

            for (var i = 0; i < waves.Count; i++)
            {
                var matches = waves[i];
                var cascadeMult = 1f + config.CascadeMultiplierStep * i;
                var multiMatchMult = matches.Count > 1 ? config.MultiMatchMultiplier : 1f;

                sb.AppendLine($"  Wave {i + 1}  cascade×{cascadeMult:F2}  multiMatch×{multiMatchMult:F2}  ({matches.Count} match(es))");

                for (var j = 0; j < matches.Count; j++)
                {
                    var match = matches[j];
                    if (false == match.TileKind.IsColor())
                        continue;

                    var shapeMult = match.Shape switch
                    {
                        MatchShape.LShape => config.LShapeMultiplier,
                        MatchShape.TShape => config.TShapeMultiplier,
                        _ => 1f
                    };

                    var raw = match.Positions.Count * shapeMult * cascadeMult * multiMatchMult;
                    sb.AppendLine($"    [{match.TileKind}] {match.Positions.Count} tiles  shape={match.Shape}×{shapeMult:F2}  → +{raw:F2}");
                }
            }

            sb.AppendLine("  == Per-kind totals ==");
            var total = 0f;
            foreach (var pair in energyByKind)
            {
                if (pair.Value <= 0f)
                    continue;

                sb.AppendLine($"  {pair.Key}: +{pair.Value:F2}");
                total += pair.Value;
            }

            sb.Append($"  Total energy generated: +{total:F2}");
            return sb.ToString();
        }

        private static void AccumulateGridDiffEnergy(TileKind[,] before, TileKind[,] after, Dictionary<TileKind, float> energy)
        {
            var width = before.GetLength(0);
            var height = before.GetLength(1);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var kindBefore = before[x, y];
                    if (false == kindBefore.IsColor())
                        continue;

                    if (after[x, y] != kindBefore)
                    {
                        energy.TryGetValue(kindBefore, out var current);
                        energy[kindBefore] = current + 1f;
                    }
                }
            }
        }
    }
}