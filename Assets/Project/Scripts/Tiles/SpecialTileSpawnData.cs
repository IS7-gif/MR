using Project.Scripts.Configs;

namespace Project.Scripts.Tiles
{
    public readonly struct SpecialTileSpawnData
    {
        public readonly TileConfig Config;
        public readonly TileType PayloadType;


        public SpecialTileSpawnData(TileConfig config, TileType payloadType = TileType.None)
        {
            Config = config;
            PayloadType = payloadType;
        }
    }
}