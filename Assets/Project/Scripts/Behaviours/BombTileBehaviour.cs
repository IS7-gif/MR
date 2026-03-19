using Project.Scripts.Services.Grid;
using Project.Scripts.Tiles;
using UnityEngine;

namespace Project.Scripts.Behaviours
{
    [CreateAssetMenu(fileName = "BombTileBehaviour", menuName = "Configs/Behaviours/Bomb")]
    public class BombTileBehaviour : TileBehaviour
    {
        [Tooltip("Grid radius of the explosion for a standard activation — destroys all tiles within this many cells")]
        [SerializeField] private int _radius = 1;

        [Tooltip("Grid radius used when two Bombs are swapped together (Bomb + Bomb combo)")]
        [SerializeField] private int _doubleRadius = 2;


        public override bool IsActivatedBySwap => true;
        public override SpecialTileKind SpecialKind => SpecialTileKind.Bomb;

        public int Radius => _radius;
        public int DoubleRadius => _doubleRadius;


        public override void OnTileDestroyed(Vector2Int gridPos, IGridManager grid)
        {
            var neighbours = grid.GetNeighboursInRadius(gridPos, _radius);
            grid.ScheduleRemove(neighbours);
        }
    }
}