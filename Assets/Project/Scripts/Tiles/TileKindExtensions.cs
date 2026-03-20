namespace Project.Scripts.Tiles
{
    public static class TileKindExtensions
    {
        public static bool IsColor(this TileKind kind) =>
            kind is >= TileKind.Red and <= TileKind.Purple;

        public static bool IsSpecial(this TileKind kind) =>
            kind >= TileKind.Bomb;
    }
}