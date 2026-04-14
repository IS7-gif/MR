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
            Application.focusChanged += OnApplicationFocusChanged;

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
            Application.focusChanged -= OnApplicationFocusChanged;
            _map?.Disable();
        }


        private void OnPressStarted(InputAction.CallbackContext ctx)
        {
            var device = ctx.control?.device;
            if (null == device)
                return;

            if (_isDragging)
                ResetDrag(true);

            if (false == TryReadCurrentPosition(device, out var currentPosition))
                return;

            _activeDragDevice = device;

            _isDragging = true;
            _lastPosition = currentPosition;
            OnDragStarted?.Invoke(_lastPosition);
        }

        public void ReanchorDrag()
        {
            if (false == _isDragging)
                return;

            if (false == TryReadCurrentPosition(_activeDragDevice, out var currentPosition))
            {
                ResetDrag(true);
                return;
            }

            _lastPosition = currentPosition;
            OnDragStarted?.Invoke(_lastPosition);
        }

        private void OnPressCanceled(InputAction.CallbackContext ctx)
        {
            var device = ctx.control?.device;
            if (_activeDragDevice != null && _activeDragDevice != device && _pressAction != null && _pressAction.IsPressed())
                return;

            ResetDrag(true);
        }

        private void OnPointPerformed(InputAction.CallbackContext ctx)
        {
            if (false == _isDragging)
                return;

            var device = ctx.control?.device;
            if (_activeDragDevice != device)
                return;

            var current = ctx.ReadValue<Vector2>();
            if (false == IsFinite(current))
            {
                ResetDrag(true);
                return;
            }

            var delta = current - _lastPosition;
            _lastPosition = current;

            if (delta.sqrMagnitude > 0.001f)
                OnDragDelta?.Invoke(delta);
        }

        private void OnApplicationFocusChanged(bool hasFocus)
        {
            if (hasFocus)
                return;

            ResetDrag(true);
        }

        private void ResetDrag(bool notifyCanceled)
        {
            if (false == _isDragging && null == _activeDragDevice)
                return;

            _isDragging = false;
            _lastPosition = default;
            _activeDragDevice = null;

            if (notifyCanceled)
                OnDragCanceled?.Invoke();
        }

        private static bool IsFinite(Vector2 position)
        {
            return float.IsFinite(position.x) && float.IsFinite(position.y);
        }

        private static bool TryReadCurrentPosition(InputDevice device, out Vector2 position)
        {
            switch (device)
            {
                case Mouse mouse:
                    position = mouse.position.ReadValue();
                    return IsFinite(position);

                case Touchscreen touchscreen:
                    position = touchscreen.primaryTouch.position.ReadValue();
                    return IsFinite(position);

                default:
                    position = default;
                    return false;
            }
        }
    }
}