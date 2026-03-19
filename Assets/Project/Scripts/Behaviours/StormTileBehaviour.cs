using Project.Scripts.Services.Grid;
using Project.Scripts.Tiles;
using UnityEngine;

namespace Project.Scripts.Behaviours
{
    [CreateAssetMenu(fileName = "StormTileBehaviour", menuName = "Configs/Behaviours/Storm")]
    public class StormTileBehaviour : TileBehaviour
    {
        public override bool IsActivatedBySwap => true;


        public override void OnTileDestroyed(Vector2Int gridPos, IGridManager grid)
        {
            var tile = grid.GetTile(gridPos);
            if (!tile)
                return;

            var targetType = tile.PayloadType;
            if (targetType == TileType.None)
                return;

            var positions = grid.GetAllOfType(targetType);
            grid.ScheduleRemove(positions);
        }
    }
}