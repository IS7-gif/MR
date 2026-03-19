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
        public SwapComboType Resolve(SpecialTileKind kindA, SpecialTileKind kindB)
        {
            if (kindA == SpecialTileKind.Storm || kindB == SpecialTileKind.Storm)
            {
                var other = kindA == SpecialTileKind.Storm ? kindB : kindA;
                return other switch
                {
                    SpecialTileKind.Storm => SwapComboType.StormStorm,
                    SpecialTileKind.Bomb => SwapComboType.StormBomb,
                    _ => SwapComboType.StormLine
                };
            }

            if (kindA == SpecialTileKind.Bomb && kindB == SpecialTileKind.Bomb)
                return SwapComboType.BombBomb;

            var aIsBomb = kindA == SpecialTileKind.Bomb;
            var bIsBomb = kindB == SpecialTileKind.Bomb;
            if (aIsBomb || bIsBomb)
                return SwapComboType.BombLine;

            return SwapComboType.LineLine;
        }
    }
}