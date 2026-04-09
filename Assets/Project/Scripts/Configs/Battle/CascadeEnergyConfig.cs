using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "CascadeEnergyConfig", menuName = "Configs/Battle/Cascade Energy Config")]
    public class CascadeEnergyConfig : ScriptableObject
    {
        [Header("Cascade Bonus")]
        [Tooltip("Прирост множителя за каждый уровень каскада. Каскад 1 = x1.0, каскад 2 = x(1.0 + шаг) и т.д.")]
        [SerializeField] private float _cascadeMultiplierStep = 0.15f;

        [Header("Multi-Match Bonus")]
        [Tooltip("Множитель энергии, если в одной волне 2 и более совпадений")]
        [SerializeField] private float _multiMatchMultiplier = 1.15f;

        [Header("Shape Bonuses")]
        [Tooltip("Множитель энергии для совпадений L-формы")]
        [SerializeField] private float _lShapeMultiplier = 1.20f;

        [Tooltip("Множитель энергии для совпадений T-формы")]
        [SerializeField] private float _tShapeMultiplier = 1.35f;


        public float CascadeMultiplierStep => _cascadeMultiplierStep;
        public float MultiMatchMultiplier => _multiMatchMultiplier;
        public float LShapeMultiplier => _lShapeMultiplier;
        public float TShapeMultiplier => _tShapeMultiplier;
    }
}