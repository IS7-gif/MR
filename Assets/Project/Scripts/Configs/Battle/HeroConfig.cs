using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Tiles;
using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "HeroConfig", menuName = "Configs/Hero Config")]
    public class HeroConfig : ScriptableObject
    {
        [Tooltip("Тип элемента тайла этого героя - определяет, каким типом энергии заполняется этот герой")]
        [SerializeField] private TileKind _kind;

        [Tooltip("Энергия, необходимая для активации способности этого героя")]
        [SerializeField] private int _maxEnergy = 10;

        [Tooltip("Действие героя при активации")]
        [SerializeField] private HeroActionType _actionType;

        [Tooltip("Количество урона (DealDamage) или восстановленного HP (HealAlly) при активации")]
        [SerializeField] private int _actionValue = 20;

        [Tooltip("Спрайт портрета в слоте героя (null = пустая рамка)")]
        [SerializeField] private Sprite _portrait;

        [Tooltip("Максимальные HP этого героя. Ноль означает, что герой бессмертен (не может получать урон)")]
        [SerializeField] private int _maxHP = 50;

        [Tooltip("Отображаемое имя героя, для будущих UI-ярлыков")]
        [SerializeField] private string _displayName;


        public TileKind Kind => _kind;
        public int MaxEnergy => _maxEnergy;
        public HeroActionType ActionType => _actionType;
        public int ActionValue => _actionValue;
        public int MaxHP => _maxHP;
        public Sprite Portrait => _portrait;
        public string DisplayName => _displayName;
    }
}