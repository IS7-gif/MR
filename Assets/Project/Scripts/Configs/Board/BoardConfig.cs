using Project.Scripts.Tiles;
using UnityEngine;
#if UNITY_EDITOR
using System;
#endif

namespace Project.Scripts.Configs.Board
{
    [CreateAssetMenu(fileName = "BoardConfig", menuName = "Configs/Board Config")]
    public class BoardConfig : ScriptableObject
    {
        [Header("Tile Grid")]
        [Tooltip("Доля ширины экрана, зарезервированная как отступ вокруг сетки тайлов (0 = без отступа, 0.5 = половина экрана)")]
        [Range(0f, 0.5f)]
        [SerializeField] private float _tilePaddingPercent = 0.166f;

        [Tooltip("Доля высоты экрана, зарезервированная для UI (счёт, кнопки и т.д.) - доска не будет расширяться в эту область")]
        [Range(0f, 0.5f)]
        [SerializeField] private float _uiReservedHeightPercent = 0.4f;

        [Tooltip("Визуальный размер тайла относительно ячейки (1 = заполняет ячейку полностью, <1 = зазоры, >1 = тайлы перекрываются прозрачными краями спрайта)")]
        [Range(0.5f, 2f)]
        [SerializeField] private float _tileScale = 1.162f;

        [Header("Frame / Background")]
        [Tooltip("Доля ширины экрана, зарезервированная как отступ вокруг рамки-фона доски. " +
                 "Должна быть меньше TilePaddingPercent, чтобы рамка выступала за тайлы. " +
                 "Равна TilePaddingPercent - рамка вплотную к тайлам. Больше - рамка меньше тайлов (нетипично).")]
        [Range(0f, 0.5f)]
        [SerializeField] private float _framePaddingPercent = 0f;

        [Tooltip("Дополнительная высота рамки-фона в Unity units поверх высоты тайловой сетки. " +
                 "0 = рамка по высоте точно под тайлы. >0 = рамка вытягивается вверх, тайловая сетка центрируется внутри.")]
        [Range(0f, 5f)]
        [SerializeField] private float _frameExtraHeight = 0.08f;

        [Header("Board Layout")]
        [Tooltip("Сколько рядов тайлов маска открывает выше рамки (>0 - скрывает спавн сверху, <0 - обрезает тайлы внутри рамки сверху)")]
        [Range(-2f, 3f)]
        [SerializeField] private float _maskTopPadding = -0.3f;

        [Tooltip("Минимальное количество тайлов в ряду/столбце для засчитывания совпадения")]
        [Range(2, 6)]
        [SerializeField] private int _minMatchLength = 3;

        [Tooltip("Максимальное соотношение ширины к высоте игровой области. На широких экранах контент обрамляется до этого соотношения (0.5 = 1:2)")]
        [Range(0.3f, 1f)]
        [SerializeField] private float _maxAspectRatio = 0.5f;

        [Tooltip("Префаб для инстанцирования каждого тайла")]
        [Space(10)]
        [SerializeField] private Tile _tilePrefab;


#if UNITY_EDITOR
        public static event Action LayoutChanged;

        
        private void OnValidate()
        {
            LayoutChanged?.Invoke();
        }
#endif

        public float TilePaddingPercent => _tilePaddingPercent;
        public float UIReservedHeightPercent => _uiReservedHeightPercent;
        public float TileScale => _tileScale;
        public float FramePaddingPercent => _framePaddingPercent;
        public float FrameExtraHeight => _frameExtraHeight;
        public float MaskTopPadding => _maskTopPadding;
        public int MinMatchLength => _minMatchLength;
        public float MaxAspectRatio => _maxAspectRatio;
        public Tile TilePrefab => _tilePrefab;
    }
}