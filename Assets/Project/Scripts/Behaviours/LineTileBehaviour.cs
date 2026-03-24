using Project.Scripts.Shared;
using Project.Scripts.Shared.Tiles;
using UnityEngine;

namespace Project.Scripts.Behaviours
{
    [CreateAssetMenu(fileName = "LineTileBehaviour", menuName = "Configs/Behaviours/Line")]
    public class LineTileBehaviour : TileBehaviour
    {
        [Tooltip("True - clears the entire row; False - clears the entire column")]
        [SerializeField] private bool _isHorizontal;


        public override bool IsActivatedBySwap => true;

        public bool IsHorizontal => _isHorizontal;


        public override void OnTileDestroyed(GridPoint gridPos, IGridState state, TileKind payloadKind)
        {
            var positions = _isHorizontal ? state.GetAllInRow(gridPos.Y) : state.GetAllInColumn(gridPos.X);
            state.ScheduleRemove(positions);
        }
    }
}