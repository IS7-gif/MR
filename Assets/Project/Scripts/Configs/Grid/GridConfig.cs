using UnityEngine;

namespace Project.Scripts.Configs.Grid
{
    [CreateAssetMenu(fileName = "GridConfig", menuName = "Configs/Grid Config")]
    public class GridConfig : ScriptableObject
    {
        [Header("Board Dimensions")]
        [Tooltip("Количество столбцов тайловой сетки")]
        [Range(3, 20)]
        [SerializeField] private int _width = 7;

        [Tooltip("Количество строк тайловой сетки")]
        [Range(3, 20)]
        [SerializeField] private int _height = 6;


        public int Width => _width;
        public int Height => _height;
    }
}
