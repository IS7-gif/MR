using Project.Scripts.Shared.Heroes;
using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "HeroConfig", menuName = "Configs/Hero Config")]
    public class HeroConfig : ScriptableObject
    {
        [Tooltip("Максимальные HP этого героя. Ноль означает, что герой бессмертен (не может получать урон)")]
        [SerializeField] private int _maxHP = 50;

        [Tooltip("Действие героя при активации")]
        [SerializeField] private HeroActionType _abilityType;

        [Tooltip("Количество урона (DealDamage) или восстановленного HP (HealAlly) при активации")]
        [SerializeField] private int _abilityPower = 20;

        [Tooltip("Стоимость активации героя из общего пула энергии стороны")]
        [SerializeField] private int _activationEnergyCost = 10;

        [Tooltip("Кулдаун повторной активации героя в секундах")]
        [SerializeField] private float _activationCooldownSeconds = 3;
        
        [Tooltip("Спрайт портрета в слоте героя (null = пустая рамка)")]
        [SerializeField] private Sprite _portrait;

        [Tooltip("Отображаемое имя героя, для будущих UI-ярлыков")]
        [SerializeField] private string _displayName;

        [Tooltip("Пассивные способности героя. Пустой список означает, что пассивных способностей нет")]
        [SerializeField] private HeroPassiveConfig[] _passiveAbilities;


        public HeroActionType AbilityType => _abilityType;
        public int AbilityPower => _abilityPower;
        public int MaxHP => _maxHP;
        public Sprite Portrait => _portrait;
        public string DisplayName => _displayName;
        public int ActivationEnergyCost => _activationEnergyCost;
        public float ActivationCooldownSeconds => _activationCooldownSeconds;
        public HeroPassiveConfig[] PassiveAbilities => _passiveAbilities;
    }
}