using System;
using Project.Scripts.Shared.Passives;
using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [Serializable]
    public class HeroPassiveConfig
    {
        [Tooltip("Тип эффекта пассивной способности")]
        [SerializeField] private PassiveAbilityKind _abilityKind;

        [Tooltip("Условие активации пассивной способности")]
        [SerializeField] private PassiveConditionKind _conditionKind;

        [Tooltip("Числовое значение, которое требуется условию активации")]
        [SerializeField] private int _conditionRequirement = 1;

        [Tooltip("Сила пассивной способности")]
        [SerializeField] private int _power;

        [Tooltip("Может ли пассивная способность активироваться повторно")]
        [SerializeField] private bool _allowMultipleActivations;

        [Tooltip("Максимум повторных активаций. Ноль означает без ограничений")]
        [SerializeField] private int _activationLimit;

        [Tooltip("Окно условия в секундах. Ноль означает, что условие не использует временное окно")]
        [SerializeField] private float _conditionWindowSeconds;


        public PassiveAbilityKind AbilityKind => _abilityKind;
        public PassiveConditionKind ConditionKind => _conditionKind;
        public int ConditionRequirement => _conditionRequirement;
        public int Power => _power;
        public bool AllowMultipleActivations => _allowMultipleActivations;
        public int ActivationLimit => _activationLimit;
        public float ConditionWindowSeconds => _conditionWindowSeconds;


        public HeroPassiveDefinition ToDefinition()
        {
            return new HeroPassiveDefinition(
                _abilityKind,
                _conditionKind,
                _conditionRequirement,
                _power,
                _allowMultipleActivations,
                _activationLimit,
                _conditionWindowSeconds);
        }
    }
}