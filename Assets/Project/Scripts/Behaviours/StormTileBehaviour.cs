using Project.Scripts.Services.Grid;
using Project.Scripts.Tiles;
using UnityEngine;

namespace Project.Scripts.Behaviours
{
    [CreateAssetMenu(fileName = "StormTileBehaviour", menuName = "Configs/Behaviours/Storm")]
    public class StormTileBehaviour : TileBehaviour
    {
        public override bool IsActivatedBySwap => true;
        public override SpecialTileKind SpecialKind => SpecialTileKind.Storm;


        public override void OnTileDestroyed(Vector2Int gridPos, IGridManager grid)
        {
            var tile = grid.GetTile(gridPos);
            if (false == tile)
                return;

            var targetType = tile.PayloadType != TileType.None
                ? tile.PayloadType
                : grid.GetMostCommonRegularType();

            if (targetType == TileType.None)
                return;

            var positions = grid.GetAllOfType(targetType);
            grid.ScheduleRemove(positions);
        }
    }
}