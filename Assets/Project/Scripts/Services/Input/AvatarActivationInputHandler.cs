using Project.Scripts.Services.EventBusSystem;
using Project.Scripts.Services.EventBusSystem.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Services.Input
{
    [RequireComponent(typeof(Button))]
    public class AvatarActivationInputHandler : MonoBehaviour
    {
        private EventBus _eventBus;
        private Button _button;

        
        private void OnDestroy()
        {
            _button?.onClick.RemoveListener(OnAvatarActivated);
        }
        

        public void Initialize(EventBus eventBus)
        {
            _eventBus = eventBus;
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnAvatarActivated);
        }

        public void SetInteractable(bool interactable)
        {
            if (_button)
                _button.interactable = interactable;
        }

        private void OnAvatarActivated()
        {
            _eventBus?.Publish(new PlayerAvatarActivatedEvent());
        }
    }
}