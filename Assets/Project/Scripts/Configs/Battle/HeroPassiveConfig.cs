using System;
using Project.Scripts.Shared.Passives;
using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [Serializable]
    public class HeroPassiveConditionConfig
    {
        [Tooltip("Тип атомарного условия")]
        [SerializeField] private PassiveConditionKind _kind;

        [Tooltip("Сколько прогресса требуется для выполнения условия")]
        [SerializeField] private int _requiredValue = 1;

        [Tooltip("Окно условия в секундах. Ноль означает, что условие не использует временное окно")]
        [SerializeField] private float _windowSeconds;


        public PassiveConditionKind Kind => _kind;
        public int RequiredValue => _requiredValue;
        public float WindowSeconds => _windowSeconds;


        public PassiveConditionDefinition ToDefinition()
        {
            return new PassiveConditionDefinition(_kind, _requiredValue, _windowSeconds);
        }
    }

    [Serializable]
    public class HeroPassiveConditionGroupConfig
    {
        [Tooltip("Как объединять атомарные условия")]
        [SerializeField] private PassiveConditionGroupOperator _operator;

        [Tooltip("Атомарные условия пассивной способности")]
        [SerializeField] private HeroPassiveConditionConfig[] _conditions;


        public PassiveConditionGroupOperator Operator => _operator;
        public HeroPassiveConditionConfig[] Conditions => _conditions;


        public PassiveConditionGroupDefinition ToDefinition()
        {
            return new PassiveConditionGroupDefinition(_operator, ToConditionDefinitions());
        }

        private PassiveConditionDefinition[] ToConditionDefinitions()
        {
            if (_conditions == null || _conditions.Length == 0)
                return Array.Empty<PassiveConditionDefinition>();

            var result = new PassiveConditionDefinition[_conditions.Length];
            for (var i = 0; i < _conditions.Length; i++)
                result[i] = _conditions[i] != null ? _conditions[i].ToDefinition() : default;

            return result;
        }
    }

    [Serializable]
    public class HeroPassiveModifierEffectConfig
    {
        [Tooltip("Какой числовой параметр меняет эффект")]
        [SerializeField] private PassiveModifierTarget _target;

        [Tooltip("Как именно меняется параметр")]
        [SerializeField] private PassiveModifierOperation _operation;

        [Tooltip("Значение модификатора")]
        [SerializeField] private float _value;


        public PassiveModifierTarget Target => _target;
        public PassiveModifierOperation Operation => _operation;
        public float Value => _value;


        public PassiveModifierEffectDefinition ToDefinition()
        {
            return new PassiveModifierEffectDefinition(_target, _operation, _value);
        }
    }

    [Serializable]
    public class HeroPassiveActionEffectConfig
    {
        [Tooltip("Тип action-эффекта")]
        [SerializeField] private PassiveActionEffectKind _kind;

        [Tooltip("Числовое значение action-эффекта")]
        [SerializeField] private float _value;

        [Tooltip("Количество/лимит action-эффекта")]
        [SerializeField] private int _count;


        public PassiveActionEffectKind Kind => _kind;
        public float Value => _value;
        public int Count => _count;


        public PassiveActionEffectDefinition ToDefinition()
        {
            return new PassiveActionEffectDefinition(_kind, _value, _count);
        }
    }

    [Serializable]
    public class HeroPassiveConfig
    {
        [Tooltip("Отображаемое имя пассивной способности")]
        [SerializeField] private string _displayName;

        [Tooltip("Группа условий активации пассивной способности")]
        [SerializeField] private HeroPassiveConditionGroupConfig _conditionGroup;

        [Tooltip("Числовые модификаторы, которые включаются после активации пассивной способности")]
        [SerializeField] private HeroPassiveModifierEffectConfig[] _modifierEffects;

        [Tooltip("Action-эффекты, которые будут исполняться после активации пассивной способности")]
        [SerializeField] private HeroPassiveActionEffectConfig[] _actionEffects;

        [Tooltip("Может ли пассивная способность активироваться повторно")]
        [SerializeField] private bool _allowMultipleActivations;

        [Tooltip("Максимум повторных активаций. Ноль означает без ограничений")]
        [SerializeField] private int _activationLimit;


        public string DisplayName => _displayName;
        public HeroPassiveConditionGroupConfig ConditionGroup => _conditionGroup;
        public HeroPassiveModifierEffectConfig[] ModifierEffects => _modifierEffects;
        public HeroPassiveActionEffectConfig[] ActionEffects => _actionEffects;
        public bool AllowMultipleActivations => _allowMultipleActivations;
        public int ActivationLimit => _activationLimit;


        public HeroPassiveDefinition ToDefinition()
        {
            return new HeroPassiveDefinition(
                _displayName,
                _conditionGroup != null ? _conditionGroup.ToDefinition() : default,
                ToModifierEffectDefinitions(),
                ToActionEffectDefinitions(),
                _allowMultipleActivations,
                _activationLimit);
        }

        private PassiveModifierEffectDefinition[] ToModifierEffectDefinitions()
        {
            if (_modifierEffects == null || _modifierEffects.Length == 0)
                return Array.Empty<PassiveModifierEffectDefinition>();

            var result = new PassiveModifierEffectDefinition[_modifierEffects.Length];
            for (var i = 0; i < _modifierEffects.Length; i++)
                result[i] = _modifierEffects[i] != null ? _modifierEffects[i].ToDefinition() : default;

            return result;
        }

        private PassiveActionEffectDefinition[] ToActionEffectDefinitions()
        {
            if (_actionEffects == null || _actionEffects.Length == 0)
                return Array.Empty<PassiveActionEffectDefinition>();

            var result = new PassiveActionEffectDefinition[_actionEffects.Length];
            for (var i = 0; i < _actionEffects.Length; i++)
                result[i] = _actionEffects[i] != null ? _actionEffects[i].ToDefinition() : default;

            return result;
        }
    }
}