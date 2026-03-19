using UnityEngine;

namespace Project.Scripts.Configs
{
    [CreateAssetMenu(fileName = "DamageConfig", menuName = "Configs/Damage Config")]
    public class DamageConfig : ScriptableObject
    {
        [Tooltip("Damage dealt by a match of 3 tiles")]
        [SerializeField] private int _match3Damage = 10;

        [Tooltip("Damage dealt by a match of 4 tiles")]
        [SerializeField] private int _match4Damage = 18;

        [Tooltip("Damage dealt by a match of 5+ tiles")]
        [SerializeField] private int _match5PlusDamage = 30;

        [Tooltip("Additional damage per tile beyond 5 in a single match")]
        [SerializeField] private int _extraTileDamage = 5;

        [Tooltip("Cascade multiplier increase per level. Cascade 1 = x1.0, cascade 2 = x(1.0 + step), etc.")]
        [SerializeField] private float _cascadeMultiplierStep = 0.2f;

        [Tooltip("Bonus multiplier applied when 2 or more matches occur in a single wave")]
        [SerializeField] private float _multiMatchBonus = 0.2f;

        [Tooltip("Damage per tile destroyed by a bomb")]
        [SerializeField] private int _bombDamagePerTile = 3;


        public int Match3Damage => _match3Damage;
        public int Match4Damage => _match4Damage;
        public int Match5PlusDamage => _match5PlusDamage;
        public int ExtraTileDamage => _extraTileDamage;
        public float CascadeMultiplierStep => _cascadeMultiplierStep;
        public float MultiMatchBonus => _multiMatchBonus;
        public int BombDamagePerTile => _bombDamagePerTile;
    }
}