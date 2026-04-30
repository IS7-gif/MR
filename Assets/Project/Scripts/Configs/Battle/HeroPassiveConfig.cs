using System;
using Project.Scripts.Shared.Passives;
using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
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

        [Tooltip("Кого выбирает action-эффект")]
        [SerializeField] private HeroPassiveActionTargetConfig _target;

        [Tooltip("Числовое значение action-эффекта")]
        [SerializeField] private float _value;

        [Tooltip("Количество/лимит action-эффекта")]
        [SerializeField] private int _count;


        public PassiveActionEffectKind Kind => _kind;
        public HeroPassiveActionTargetConfig Target => _target;
        public float Value => _value;
        public int Count => _count;


        public PassiveActionEffectDefinition ToDefinition()
        {
            return new PassiveActionEffectDefinition(
                _kind,
                _value,
                _count,
                _target != null ? _target.ToDefinition() : default);
        }
    }

    [Serializable]
    public class HeroPassiveActionTargetConfig
    {
        [Tooltip("Относительно владельца пассивки: союзники, враги или обе стороны")]
        [SerializeField] private PassiveUnitTargetTeam _team;

        [Tooltip("Какие типы юнитов участвуют в выборе")]
        [SerializeField] private PassiveUnitTargetKind _unitKind;

        [Tooltip("Как выбрать цели из подходящего пула")]
        [SerializeField] private PassiveUnitSelectionMode _selectionMode;

        [Tooltip("Зарезервированное числовое поле для action-эффекта")]
        [SerializeField] private int _count;

        [Tooltip("Исключать владельца пассивки из пула целей")]
        [SerializeField] private bool _excludeOwner;


        public PassiveUnitTargetTeam Team => _team;
        public PassiveUnitTargetKind UnitKind => _unitKind;
        public PassiveUnitSelectionMode SelectionMode => _selectionMode;
        public int Count => _count;
        public bool ExcludeOwner => _excludeOwner;


        public PassiveUnitTargetDefinition ToDefinition()
        {
            return new PassiveUnitTargetDefinition(_team, _unitKind, _selectionMode, _count, _excludeOwner);
        }
    }

    [Serializable]
    public class HeroPassiveConfig
    {
        [Tooltip("Отображаемое имя пассивной способности")]
        [SerializeField] private string _displayName;

        [Tooltip("Событие, которое двигает пассивку к активации")]
        [SerializeField] private PassiveTriggerKind _triggerKind = PassiveTriggerKind.OwnerActivatedInHeroPhase;

        [Tooltip("Сколько раз должен случиться триггер для активации")]
        [SerializeField] private int _requiredTriggerCount = 1;

        [Tooltip("Числовые модификаторы, которые включаются после активации пассивной способности")]
        [SerializeField] private HeroPassiveModifierEffectConfig[] _modifierEffects;

        [Tooltip("Action-эффекты, которые будут исполняться после активации пассивной способности")]
        [SerializeField] private HeroPassiveActionEffectConfig[] _actionEffects;

        [Tooltip("Может ли пассивная способность активироваться повторно, пока ее эффект уже активен")]
        [SerializeField] private bool _canActivateWhileActive;

        [Tooltip("Максимум активаций за бой. Ноль означает без ограничений")]
        [SerializeField] private int _maxActivations;

        [Tooltip("Сколько раундов действует активный эффект. Ноль означает до конца боя")]
        [SerializeField] private int _activeDurationRounds;


        public string DisplayName => _displayName;
        public PassiveTriggerKind TriggerKind => _triggerKind;
        public int RequiredTriggerCount => _requiredTriggerCount;
        public HeroPassiveModifierEffectConfig[] ModifierEffects => _modifierEffects;
        public HeroPassiveActionEffectConfig[] ActionEffects => _actionEffects;
        public bool CanActivateWhileActive => _canActivateWhileActive;
        public int MaxActivations => _maxActivations;
        public int ActiveDurationRounds => _activeDurationRounds;


        public HeroPassiveDefinition ToDefinition()
        {
            return new HeroPassiveDefinition(
                _displayName,
                _triggerKind,
                _requiredTriggerCount,
                ToModifierEffectDefinitions(),
                ToActionEffectDefinitions(),
                _canActivateWhileActive,
                _maxActivations,
                _activeDurationRounds);
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