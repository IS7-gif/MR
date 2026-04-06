using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "MatchDamageConfig", menuName = "Configs/Battle/Match Damage Config")]
    public class MatchDamageConfig : ScriptableObject
    {
        [Header("Base Match Damage")]
        [Tooltip("Урон от совпадения 3 тайлов")]
        [SerializeField] private int _match3Damage = 10;

        [Tooltip("Урон от совпадения 4 тайлов")]
        [SerializeField] private int _match4Damage = 18;

        [Tooltip("Урон от совпадения 5+ тайлов")]
        [SerializeField] private int _match5PlusDamage = 30;

        [Tooltip("Дополнительный урон за каждый тайл сверх 5 в одном совпадении")]
        [SerializeField] private int _extraTileDamage = 5;

        [Header("Shape Bonuses")]
        [Tooltip("Фиксированный бонус урона для совпадений L-формы")]
        [SerializeField] private int _lShapeBonus = 15;

        [Tooltip("Фиксированный бонус урона для совпадений T-формы")]
        [SerializeField] private int _tShapeBonus = 25;

        [Header("Wave Multipliers")]
        [Tooltip("Увеличение множителя каскада на каждый уровень. Каскад 1 = x1.0, каскад 2 = x(1.0 + шаг) и т.д.")]
        [SerializeField] private float _cascadeMultiplierStep = 0.2f;

        [Tooltip("Бонусный множитель при 2 и более совпадениях в одной волне")]
        [SerializeField] private float _multiMatchBonus = 0.2f;

        [Header("Bomb")]
        [Tooltip("Урон за каждый тайл, уничтоженный бомбой")]
        [SerializeField] private int _bombDamagePerTile = 3;

        [Header("Energy")]
        [Tooltip("Энергия от совпадения 3 тайлов")]
        [SerializeField] private int _match3Energy = 1;

        [Tooltip("Энергия от совпадения 4 тайлов")]
        [SerializeField] private int _match4Energy = 3;

        [Tooltip("Энергия от совпадения 5+ тайлов")]
        [SerializeField] private int _match5PlusEnergy = 5;


        public int Match3Damage => _match3Damage;
        public int Match4Damage => _match4Damage;
        public int Match5PlusDamage => _match5PlusDamage;
        public int ExtraTileDamage => _extraTileDamage;
        public int LShapeBonus => _lShapeBonus;
        public int TShapeBonus => _tShapeBonus;
        public float CascadeMultiplierStep => _cascadeMultiplierStep;
        public float MultiMatchBonus => _multiMatchBonus;
        public int BombDamagePerTile => _bombDamagePerTile;
        public int Match3Energy => _match3Energy;
        public int Match4Energy => _match4Energy;
        public int Match5PlusEnergy => _match5PlusEnergy;
    }
}