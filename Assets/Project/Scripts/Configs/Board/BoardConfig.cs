using Project.Scripts.Tiles;
using UnityEngine;

namespace Project.Scripts.Configs.Board
{
    [CreateAssetMenu(fileName = "BoardConfig", menuName = "Configs/Board Config")]
    public class BoardConfig : ScriptableObject
    {
        [Tooltip("Доля ширины экрана, зарезервированная как отступ с каждой стороны (0 = без отступа, 0.5 = половина экрана)")]
        [SerializeField] [Range(0f, 0.5f)] private float _boardPaddingPercent = 0.08f;

        [Tooltip("Доля высоты экрана, зарезервированная для UI (счёт, кнопки и т.д.) - доска не будет расширяться в эту область")]
        [SerializeField] [Range(0f, 0.5f)] private float _uiReservedHeightPercent = 0.4f;

        [Tooltip("Визуальный размер тайла относительно ячейки (1 = заполняет ячейку полностью, <1 = зазоры между тайлами)")]
        [SerializeField] [Range(0.5f, 1f)] private float _tileScale = 0.85f;

        [Tooltip("Дополнительное пространство в единицах Unity вокруг доски для спрайта рамки")]
        [SerializeField] private float _framePadding = 0.1f;

        [Tooltip("Сколько дополнительных рядов тайлов маска спавна открывает выше доски (скрывает тайлы, появляющиеся сверху)")]
        [SerializeField] private float _maskTopPadding = 2f;

        [Tooltip("Фиксированный отступ между нижним краем экрана и нижним краем доски, в единицах ячеек (1 = высота одного тайла). Одинаков для всех разрешений.")]
        [SerializeField] [Range(0f, 5f)] private float _boardBottomPaddingCells = 0.65f;

        [Tooltip("Минимальное количество тайлов в ряду/столбце для засчитывания совпадения")]
        [SerializeField] [Range(2, 6)] private int _minMatchLength = 3;

        [Tooltip("Максимальное соотношение ширины к высоте игровой области. На широких экранах контент обрамляется до этого соотношения (0.5 = 1:2)")]
        [SerializeField] [Range(0.3f, 1f)] private float _maxAspectRatio = 0.5f;

        [Tooltip("Префаб для инстанцирования каждого тайла")]
        [SerializeField] private Tile _tilePrefab;


        public float BoardPaddingPercent => _boardPaddingPercent;
        public float UIReservedHeightPercent => _uiReservedHeightPercent;
        public float TileScale => _tileScale;
        public float FramePadding => _framePadding;
        public float MaskTopPadding => _maskTopPadding;
        public float BoardBottomPaddingCells => _boardBottomPaddingCells;
        public int MinMatchLength => _minMatchLength;
        public float MaxAspectRatio => _maxAspectRatio;
        public Tile TilePrefab => _tilePrefab;
    }
}