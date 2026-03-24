using Project.Scripts.Shared.Grid;
using Project.Scripts.Shared.Tiles;

namespace Project.Scripts.Shared.Damage
{
    public readonly struct MatchInfo
    {
        public readonly MatchShape Shape;
        public readonly TileKind TileKind;
        public readonly int MaxLineLength;
        public readonly int TileCount;
        public readonly GridPoint Center;
        public readonly int Damage;
        public readonly int EnergyGenerated;


        public MatchInfo(MatchShape shape, TileKind tileKind, int maxLineLength,
            int tileCount, GridPoint center, int damage, int energyGenerated)
        {
            Shape = shape;
            TileKind = tileKind;
            MaxLineLength = maxLineLength;
            TileCount = tileCount;
            Center = center;
            Damage = damage;
            EnergyGenerated = energyGenerated;
        }
    }
}