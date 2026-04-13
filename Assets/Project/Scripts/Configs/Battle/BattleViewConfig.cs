using UnityEngine;
#if UNITY_EDITOR
using System;
#endif

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "BattleViewConfig", menuName = "Configs/Battle/Battle View Config")]
    public class BattleViewConfig : ScriptableObject
    {
        [Tooltip("Отступ в мировых координатах между верхним краем доски и нижним краем BattleHUDView. 0 — вплотную.")]
        [Range(0f, 2f)]
        [SerializeField] private float _battleHUDBottomOffset = 0.4f;
        
        [Tooltip("Префаб компонента BattleHUDView - корень отображения боя в мировом пространстве")]
        [Space(10)]
        [SerializeField] private GameObject _battleHUDViewPrefab;


#if UNITY_EDITOR
        public static event Action LayoutChanged;

        private void OnValidate() => LayoutChanged?.Invoke();
#endif

        public GameObject BattleHUDViewPrefab => _battleHUDViewPrefab;
        public float BattleHUDBottomOffset => _battleHUDBottomOffset;
    }
}