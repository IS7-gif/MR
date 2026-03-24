using Project.Scripts.Shared;
using Project.Scripts.Shared.Tiles;
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

        public int Radius => _radius;
        public int DoubleRadius => _doubleRadius;


        public override void OnTileDestroyed(GridPoint gridPos, IGridState state, TileKind payloadKind)
        {
            var neighbours = state.GetNeighboursInRadius(gridPos, _radius);
            state.ScheduleRemove(neighbours);
        }
    }
}