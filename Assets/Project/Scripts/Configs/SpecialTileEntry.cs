using System;
using UnityEngine;

namespace Project.Scripts.Configs
{
    [Serializable]
    public class SpecialTileEntry
    {
        [Tooltip("Match condition that triggers creation of this special tile")]
        [SerializeField] private SpecialTileCondition _condition;

        [Tooltip("Tile config to spawn when the condition is met (leave empty to disable this rule)")]
        [SerializeField] private TileConfig _tileToSpawn;

        [Tooltip("Where on the board the special tile appears: at the center of the match shape, or at the swap pivot position")]
        [SerializeField] private SpecialTileSpawnPosition _spawnPosition;


        public SpecialTileCondition Condition => _condition;
        public TileConfig TileToSpawn => _tileToSpawn;
        public SpecialTileSpawnPosition SpawnPosition => _spawnPosition;
    }
}