using Project.Scripts.Shared.Heroes;
using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "AvatarConfig", menuName = "Configs/Battle/Avatar Config")]
    public class AvatarConfig : ScriptableObject
    {
        [Tooltip("Максимальные HP аватара. Ноль означает бессмертие")]
        [SerializeField] private int _maxHP = 550;

        [Tooltip("Максимальный заряд энергии аватара")]
        [SerializeField] private int _maxEnergy = 110;

        [Tooltip("Действие аватара при активации")]
        [SerializeField] private HeroActionType _abilityType;

        [Tooltip("Количество урона (DealDamage) или восстановленного HP (HealAlly) при активации")]
        [SerializeField] private int _abilityPower = 80;

        [Header("Avatar Slot Energy Formula")]
        [Tooltip("Множитель энергии для тайлов, соответствующих типу ячейки аватара")]
        [SerializeField] private float _primaryTileMultiplier = 4;

        [Tooltip("Множитель энергии для тайлов, не совпадающих с типом ячейки аватара")]
        [SerializeField] private float _secondaryTileMultiplier = 1f;


        public int MaxHP => _maxHP;
        public int MaxEnergy => _maxEnergy;
        public HeroActionType AbilityType => _abilityType;
        public int AbilityPower => _abilityPower;
        public float PrimaryTileMultiplier => _primaryTileMultiplier;
        public float SecondaryTileMultiplier => _secondaryTileMultiplier;
    }
}