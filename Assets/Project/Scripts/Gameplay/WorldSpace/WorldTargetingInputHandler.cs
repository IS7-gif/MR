using Project.Scripts.Services.Combat;
using Project.Scripts.Services.Input;
using Project.Scripts.Shared.Heroes;
using UnityEngine;

namespace Project.Scripts.Gameplay.WorldSpace
{
    public class WorldTargetingInputHandler : MonoBehaviour
    {
        private const float OffsetPx = 20f;


        private IInputService _input;
        private WorldTargetingRegistry _registry;
        private IAbilityExecutionService _abilityExecution;
        private Camera _cam;
        private IWorldTargetable _source;
        private IWorldTargetable _target;
        private Vector2 _currentScreenPos;


        private void OnDestroy()
        {
            Unsubscribe();
        }


        public void Init(
            IInputService input,
            WorldTargetingRegistry registry,
            IAbilityExecutionService abilityExecution,
            Camera cam)
        {
            Unsubscribe();
            _input = input;
            _registry = registry;
            _abilityExecution = abilityExecution;
            _cam = cam;

            _input.OnDragStarted += HandleDragStarted;
            _input.OnDragDelta += HandleDragDelta;
            _input.OnDragCanceled += HandleDragCanceled;
        }


        private void HandleDragStarted(Vector2 screenPos)
        {
            _currentScreenPos = screenPos;

            var unit = _registry.FindAtPosition(screenPos, _cam, OffsetPx);

            if (unit == null || false == unit.IsReadySource || unit.Descriptor.Side != BattleSide.Player)
                return;

            _source = unit;
            _source.SetSourceHighlight(true);
        }

        private void HandleDragDelta(Vector2 screenDelta)
        {
            if (null == _source)
                return;

            _currentScreenPos += screenDelta;

            var candidate = _registry.FindAtPosition(_currentScreenPos, _cam, OffsetPx);
            var valid = candidate != null
                        && candidate != _source
                        && candidate.IsValidTarget(_source.Descriptor);

            if (_target != null && _target != candidate)
            {
                _target.SetTargetHighlight(false, default);
                _target = null;
            }

            if (valid)
            {
                _target = candidate;
                _target.SetTargetHighlight(true, _source.Descriptor.ActionType);
            }
        }

        private void HandleDragCanceled()
        {
            if (_source != null && _target != null)
                _abilityExecution.Execute(_source.Descriptor, _target.Descriptor);

            _source?.SetSourceHighlight(false);
            _target?.SetTargetHighlight(false, default);
            _source = null;
            _target = null;
        }

        private void Unsubscribe()
        {
            if (null == _input)
                return;

            _input.OnDragStarted -= HandleDragStarted;
            _input.OnDragDelta -= HandleDragDelta;
            _input.OnDragCanceled -= HandleDragCanceled;
        }
    }
}