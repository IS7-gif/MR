using UnityEngine;

namespace Project.Scripts.Services.Input
{
    public readonly struct SwapRequest
    {
        public readonly Vector2Int From;
        public readonly Vector2Int To;

        // The position where the player's dragged tile lands - used as spawn point for future special tiles (Match-4, Match-5)
        public readonly Vector2Int PivotPosition;


        public SwapRequest(Vector2Int from, Vector2Int to)
        {
            From = from;
            To = to;
            PivotPosition = to;
        }
    }
}