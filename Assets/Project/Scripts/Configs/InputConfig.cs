using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Scripts.Configs
{
    [CreateAssetMenu(fileName = "InputConfig", menuName = "Configs/Input Config")]
    public class InputConfig : ScriptableObject
    {
        [Tooltip("Ассет действий Unity Input System, определяющий все привязки ввода")]
        [SerializeField] private InputActionAsset _inputActionAsset;

        [Tooltip("Минимальное расстояние перетаскивания в мировых единицах для определения направления обмена")]
        [SerializeField] private float _worldDragThreshold = 0.03f;

        [Tooltip("При разблокировке доски (конец каскада) автоматически продолжить удерживаемый свайп без отпускания пальца")]
        [SerializeField] private bool _reanchorOnUnlock = true;


        public InputActionAsset InputActionAsset => _inputActionAsset;
        public float WorldDragThreshold => _worldDragThreshold;
        public bool ReanchorOnUnlock => _reanchorOnUnlock;
    }
}