using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Project.Scripts.Configs;
using Project.Scripts.Tiles;
using UnityEngine;

namespace Project.Scripts.Services.Grid
{
    public interface IGridManager
    {
        Tile GetTile(Vector2Int pos);
        void SetTile(Vector2Int pos, Tile tile);
        void ClearTile(Vector2Int pos);
        bool IsValidPosition(Vector2Int pos);
        Vector3 GridToWorld(Vector2Int gridPos);
        Vector2Int WorldToGrid(Vector3 worldPos);
        TileType[,] GetGridState();
        TileConfig ResolveRegularTile();
        UniTask PopulateGrid();
        UniTask RemoveMatches(List<MatchResult> matches, Dictionary<Vector2Int, SpecialTileSpawnData> specialPlacements);
        UniTask SwapTiles(Vector2Int from, Vector2Int to);
        List<Vector2Int> GetNeighboursInRadius(Vector2Int center, int radius);
        List<Vector2Int> GetAllInRow(int y);
        List<Vector2Int> GetAllInColumn(int x);
        List<Vector2Int> GetAllOfType(TileType type);
        void ScheduleRemove(List<Vector2Int> positions);
        void SetOrigin(Vector3 origin);
        UniTask ActivateBySwap(Vector2Int pos);
        UniTask ShuffleGrid();
        void ForceInjectMove();
    }
}