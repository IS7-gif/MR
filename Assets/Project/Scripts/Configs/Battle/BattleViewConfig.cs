using UnityEngine;
#if UNITY_EDITOR
using System;
#endif

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "BattleViewConfig", menuName = "Configs/Battle/Battle View Config")]
    public class BattleViewConfig : ScriptableObject
    {
        [Tooltip("Смещение по Y в мировых координатах, добавляемое выше верхнего края доски для позиционирования корня боевого HUD")]
        [Range(0f, 1f)]
        [SerializeField] private float _battleAreaTopPadding = 0.4f;
        
        [Tooltip("Префаб компонента BattleHUDView - корень отображения боя в мировом пространстве")]
        [Space(10)]
        [SerializeField] private GameObject _battleHUDViewPrefab;


#if UNITY_EDITOR
        public static event Action LayoutChanged;

        private void OnValidate() => LayoutChanged?.Invoke();
#endif

        public GameObject BattleHUDViewPrefab => _battleHUDViewPrefab;
        public float BattleAreaTopPadding => _battleAreaTopPadding;
    }
}