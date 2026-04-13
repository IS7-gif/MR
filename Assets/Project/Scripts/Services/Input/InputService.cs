using System;
using Cysharp.Threading.Tasks;
using Project.Scripts.Configs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Scripts.Services.Input
{
    public class InputService : IInputService, IDisposable
    {
        public event Action<Vector2> OnDragStarted;
        public event Action<Vector2> OnDragDelta;
        public event Action OnDragCanceled;


        private readonly InputConfig _config;
        private InputAction _pressAction;
        private InputAction _pointAction;
        private InputActionMap _map;
        private bool _isDragging;
        private bool _dragStarted;
        private Vector2 _lastPosition;
        private InputDevice _activeDragDevice;


        public InputService(InputConfig config)
        {
            _config = config;
        }

        public UniTask InitAsync()
        {
            var asset = _config.InputActionAsset;
            if (!asset)
            {
                Debug.LogError("InputService: InputActionAsset is null!");
                return UniTask.CompletedTask;
            }

            _map = asset.FindActionMap("Gameplay");
            if (null == _map)
            {
                Debug.LogError("InputService: 'Gameplay' action map not found!");
                return UniTask.CompletedTask;
            }

            _pressAction = _map.FindAction("Press");
            _pointAction = _map.FindAction("Point");

            if (null == _pressAction || null == _pointAction)
            {
                Debug.LogError("InputService: 'Press' or 'Point' action not found in Gameplay map!");
                return UniTask.CompletedTask;
            }

            _pressAction.started += OnPressStarted;
            _pressAction.canceled += OnPressCanceled;
            _pointAction.performed += OnPointPerformed;

            asset.bindingMask = null;
            _map.Enable();
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            if (null == _pressAction)
                return;

            _pressAction.started -= OnPressStarted;
            _pressAction.canceled -= OnPressCanceled;
            _pointAction.performed -= OnPointPerformed;
            _map?.Disable();
        }


        private void OnPressStarted(InputAction.CallbackContext ctx)
        {
            var device = ctx.control?.device;
            if (null == device)
                return;

            if (_isDragging)
                return;

            _activeDragDevice = device;
            _isDragging = true;
            _dragStarted = false;
        }

        public void ReanchorDrag()
        {
            if (false == _isDragging)
                return;

            _dragStarted = false;
        }

        private void OnPressCanceled(InputAction.CallbackContext ctx)
        {
            var device = ctx.control?.device;
            if (_activeDragDevice != null && _activeDragDevice != device)
                return;

            _isDragging = false;
            _dragStarted = false;
            _activeDragDevice = null;
            OnDragCanceled?.Invoke();
        }

        private void OnPointPerformed(InputAction.CallbackContext ctx)
        {
            if (false == _isDragging)
                return;

            var device = ctx.control?.device;
            if (_activeDragDevice != device)
                return;

            var current = ctx.ReadValue<Vector2>();

            if (false == _dragStarted)
            {
                _dragStarted = true;
                _lastPosition = current;
                OnDragStarted?.Invoke(_lastPosition);
                return;
            }

            var delta = current - _lastPosition;
            _lastPosition = current;

            if (delta.sqrMagnitude > 0.001f)
                OnDragDelta?.Invoke(delta);
        }
    }
}