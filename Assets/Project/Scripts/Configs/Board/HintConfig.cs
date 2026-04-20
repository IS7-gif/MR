using UnityEngine;

namespace Project.Scripts.Configs.Board
{
    [CreateAssetMenu(fileName = "HintConfig", menuName = "Configs/Board/HintConfig")]
    public class HintConfig : ScriptableObject
    {
        [Tooltip("Секунд без успешного матча до активации подсказки")]
        [SerializeField] private float _idleTimeThreshold = 5f;

        [Tooltip("Показывать подсветку под тайлом")]
        [SerializeField] private bool _showGlow = true;

        [Tooltip("Анимировать пульсацию тайла")]
        [SerializeField] private bool _animatePulse = true;

        [Tooltip("Максимальный scale тайла во время пульса (множитель от базового)")]
        [SerializeField] private float _pulseScaleMax = 1.15f;

        [Tooltip("Длительность одного импульса (вверх + вниз), сек")]
        [SerializeField] private float _pulseDuration = 0.3f;

        [Tooltip("Пауза между импульсами, сек")]
        [SerializeField] private float _pauseBetweenPulses = 0.8f;


        public float IdleTimeThreshold => _idleTimeThreshold;
        public bool ShowGlow => _showGlow;
        public bool AnimatePulse => _animatePulse;
        public float PulseScaleMax => _pulseScaleMax;
        public float PulseDuration => _pulseDuration;
        public float PauseBetweenPulses => _pauseBetweenPulses;
    }
}