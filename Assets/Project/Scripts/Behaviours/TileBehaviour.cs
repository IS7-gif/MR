using Project.Scripts.Services.Grid;
using Project.Scripts.Tiles;
using UnityEngine;

namespace Project.Scripts.Behaviours
{
    public abstract class TileBehaviour : ScriptableObject
    {
        // True if this tile activates when the player swaps it (Bomb, Line, Storm).
        public virtual bool IsActivatedBySwap => false;

        // Identifies this behaviour for combo resolution; None for regular tiles.
        public virtual SpecialTileKind SpecialKind => SpecialTileKind.None;


        public abstract void OnTileDestroyed(Vector2Int gridPos, IGridManager grid);
    }
}
