using System;
using Project.Scripts.Shared.Tiles;
using UnityEngine;

namespace Project.Scripts.Configs.Board
{
    [Serializable]
    public class SpecialTileEntry
    {
        [Tooltip("Условие совпадения, запускающее создание этого специального тайла")]
        [SerializeField] private SpecialTileCondition _condition;

        [Tooltip("Вид специального тайла для спавна при выполнении условия")]
        [SerializeField] private TileKind _tileKind;

        [Tooltip("Где на доске появляется специальный тайл: в центре фигуры совпадения или в точке поворота обмена")]
        [SerializeField] private SpecialTileSpawnPosition _spawnPosition;


        public SpecialTileCondition Condition => _condition;
        public TileKind TileKind => _tileKind;
        public SpecialTileSpawnPosition SpawnPosition => _spawnPosition;
    }
}