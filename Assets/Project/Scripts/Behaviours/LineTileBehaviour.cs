using Project.Scripts.Services.Grid;
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


        public override void OnTileDestroyed(Vector2Int gridPos, IGridManager grid)
        {
            var positions = _isHorizontal ? grid.GetAllInRow(gridPos.y) : grid.GetAllInColumn(gridPos.x);
            grid.ScheduleRemove(positions);
        }
    }
}