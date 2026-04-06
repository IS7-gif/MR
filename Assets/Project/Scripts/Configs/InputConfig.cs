using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Scripts.Configs
{
    [CreateAssetMenu(fileName = "InputConfig", menuName = "Configs/Input Config")]
    public class InputConfig : ScriptableObject
    {
        [Tooltip("Ассет действий Unity Input System, определяющий все привязки ввода")]
        [SerializeField] private InputActionAsset _inputActionAsset;

        [Tooltip("Минимальное расстояние перетаскивания в пикселях для регистрации свайпа")]
        [SerializeField] private float _screenDragThresholdPixels = 10f;

        [Tooltip("Минимальное расстояние перетаскивания в мировых единицах для определения направления обмена")]
        [SerializeField] private float _worldDragThreshold = 0.03f;

        
        public InputActionAsset InputActionAsset => _inputActionAsset;
        public float ScreenDragThresholdPixels => _screenDragThresholdPixels;
        public float WorldDragThreshold => _worldDragThreshold;
    }
}