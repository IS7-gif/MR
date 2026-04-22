using Project.Scripts.Shared.Heroes;
using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "AvatarConfig", menuName = "Configs/Battle/Avatar Config")]
    public class AvatarConfig : ScriptableObject
    {
        [Tooltip("Максимальные HP аватара. Ноль означает бессмертие")]
        [SerializeField] private int _maxHP = 550;

        [Tooltip("Стоимость активации аватара из общего пула энергии стороны")]
        [SerializeField] private int _activationEnergyCost = 110;

        [Tooltip("Действие аватара при активации")]
        [SerializeField] private HeroActionType _abilityType;

        [Tooltip("Количество урона (DealDamage) или восстановленного HP (HealAlly) при активации")]
        [SerializeField] private int _abilityPower = 80;

        [Tooltip("Portrait sprite displayed in the avatar slot (null = empty frame)")]
        [SerializeField] private Sprite _portrait;

        [Tooltip("Кулдаун повторной активации аватара в секундах")]
        [SerializeField] private float _activationCooldownSeconds = 3;


        public int MaxHP => _maxHP;
        public int ActivationEnergyCost => _activationEnergyCost;
        public HeroActionType AbilityType => _abilityType;
        public int AbilityPower => _abilityPower;
        public Sprite Portrait => _portrait;
        public float ActivationCooldownSeconds => _activationCooldownSeconds;
    }
}