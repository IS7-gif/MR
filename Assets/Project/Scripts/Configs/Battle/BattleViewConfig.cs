using UnityEngine;
#if UNITY_EDITOR
using System;
#endif

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "BattleViewConfig", menuName = "Configs/Battle/Battle View Config")]
    public class BattleViewConfig : ScriptableObject
    {
        [Tooltip("Отступ между нижним краем экрана и нижним краем battle-world layout, в единицах frame-ячейки")]
        [Range(0f, 5f)]
        [SerializeField] private float _battleWorldBottomPadding = 0.15f;


#if UNITY_EDITOR
        public static event Action LayoutChanged;

        private void OnValidate() => LayoutChanged?.Invoke();
#endif

        public float BattleWorldBottomPadding => _battleWorldBottomPadding;
    }
}