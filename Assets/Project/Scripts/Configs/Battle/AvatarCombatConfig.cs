using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "AvatarCombatConfig", menuName = "Configs/Battle/Avatar Combat Config")]
    public class AvatarCombatConfig : ScriptableObject
    {
        [Tooltip("Максимальный заряд аватара игрока и врага")]
        [SerializeField] private int _maxAvatarCharge = 110;

        [Header("Avatar Slot Energy Formula")]
        [Tooltip("Множитель энергии для тайлов, соответствующих типу ячейки аватара")]
        [SerializeField] private float _primaryTileMultiplier = 1.0f;

        [Tooltip("Множитель энергии для тайлов, не совпадающих с типом ячейки аватара")]
        [SerializeField] private float _secondaryTileMultiplier = 0.25f;


        public int MaxAvatarCharge => _maxAvatarCharge;
        public float PrimaryTileMultiplier => _primaryTileMultiplier;
        public float SecondaryTileMultiplier => _secondaryTileMultiplier;
    }
}