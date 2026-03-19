using Project.Scripts.Services.Grid;
using UnityEngine;

namespace Project.Scripts.Behaviours
{
    [CreateAssetMenu(fileName = "DefaultTileBehaviour", menuName = "Configs/Behaviours/Default")]
    public class DefaultTileBehaviour : TileBehaviour
    {
        public override void OnTileDestroyed(Vector2Int gridPos, IGridManager grid) { }
    }
}