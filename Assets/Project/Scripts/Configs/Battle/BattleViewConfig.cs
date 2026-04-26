using UnityEngine;
#if UNITY_EDITOR
using System;
#endif

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "BattleViewConfig", menuName = "Configs/Battle/Battle View Config")]
    public class BattleViewConfig : ScriptableObject
    {
        [Header("Vertical Layout Gaps")]
        [Tooltip("Зазор между верхним краем доски матчинга и нижним краем блока энергии игрока, в единицах cellSize")]
        [Range(0f, 2f)]
        [SerializeField] private float _gapBoardToPlayerEnergy = 0.15f;

        [Tooltip("Зазор между верхним краем блока энергии игрока и нижним краем блока энергии врага, в единицах cellSize")]
        [Range(0f, 2f)]
        [SerializeField] private float _gapPlayerEnergyToEnemyEnergy = 0.1f;

        [Tooltip("Зазор между верхним краем блока энергии врага и нижним краем боевого поля, в единицах cellSize")]
        [Range(0f, 2f)]
        [SerializeField] private float _gapEnemyEnergyToBattleField = 0.15f;


#if UNITY_EDITOR
        public static event Action LayoutChanged;

        private void OnValidate()
        {
            LayoutChanged?.Invoke();
        }
#endif

        public float GapBoardToPlayerEnergy => _gapBoardToPlayerEnergy;
        public float GapPlayerEnergyToEnemyEnergy => _gapPlayerEnergyToEnemyEnergy;
        public float GapEnemyEnergyToBattleField => _gapEnemyEnergyToBattleField;
    }
}