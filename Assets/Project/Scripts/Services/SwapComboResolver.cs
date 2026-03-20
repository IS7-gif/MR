using Project.Scripts.Tiles;

namespace Project.Scripts.Services
{
    public enum SwapComboType
    {
        StormStorm,
        StormBomb,
        StormLine,
        BombBomb,
        BombLine,
        LineLine
    }


    public class SwapComboResolver
    {
        public SwapComboType Resolve(TileKind kindA, TileKind kindB)
        {
            if (kindA == TileKind.Storm || kindB == TileKind.Storm)
            {
                var other = kindA == TileKind.Storm ? kindB : kindA;
                return other switch
                {
                    TileKind.Storm => SwapComboType.StormStorm,
                    TileKind.Bomb => SwapComboType.StormBomb,
                    _ => SwapComboType.StormLine
                };
            }

            if (kindA == TileKind.Bomb && kindB == TileKind.Bomb)
                return SwapComboType.BombBomb;

            var aIsBomb = kindA == TileKind.Bomb;
            var bIsBomb = kindB == TileKind.Bomb;
            if (aIsBomb || bIsBomb)
                return SwapComboType.BombLine;

            return SwapComboType.LineLine;
        }
    }
}