using System;
using Project.Scripts.Shared.Tiles;
using UnityEngine;

namespace Project.Scripts.Configs.Board
{
    [CreateAssetMenu(fileName = "TileKindPaletteConfig", menuName = "Configs/TileKindPalette Config")]
    public class TileKindPaletteConfig : ScriptableObject
    {
        [Tooltip("Таблица цветов для каждого типа элемента тайла")]
        [SerializeField] private TileKindColorEntry[] _entries;


        public Color GetColor(TileKind kind, Color fallback = default)
        {
            for (var i = 0; i < _entries.Length; i++)
                if (_entries[i].Kind == kind)
                    return _entries[i].Color;

            return fallback;
        }
    }
    
    
    [Serializable]
    public struct TileKindColorEntry
    {
        [Tooltip("Тип элемента тайла")]
        public TileKind Kind;

        [Tooltip("Цвет, связанный с этим типом элемента - используется для окраски фона слотов героев")]
        public Color Color;
    }
}