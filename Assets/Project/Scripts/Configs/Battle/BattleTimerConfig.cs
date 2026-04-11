using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "BattleTimerConfig", menuName = "Configs/Battle/Battle Timer Config")]
    public class BattleTimerConfig : ScriptableObject
    {
        [Tooltip("Общая длительность боя в секундах до начала Овертайма")]
        [SerializeField] private float _battleDuration = 180f;
        
        [Tooltip("Порог оставшегося времени в секундах, при котором начинается фаза эскалации боя - запускает объявление и игровые модификаторы")]
        [SerializeField] private float _escalationThreshold = 30f;

        [Tooltip("Фиксированный урон в секунду, применяемый к каждому герою во время Овертайма - чем больше текущее HP, тем дольше выживание")]
        [SerializeField] private int _heroDrainPerSecond = 200;

        [Tooltip("Фиксированный урон в секунду, применяемый к открытому аватару во время Овертайма")]
        [SerializeField] private int _avatarDrainPerSecond = 150;

        [Tooltip("Интервал в секундах между тиками урона истощения - меньшие значения дают более плавную анимацию полосы HP")]
        [SerializeField] private float _drainTickInterval = 0.1f;

        [Tooltip("Оставшееся время в секундах, при котором начинается посекундный обратный отсчёт (например, 5 показывает 5, 4, 3, 2, 1)")]
        [SerializeField] private int _countdownThreshold = 5;


        public float BattleDuration => _battleDuration;
        public int HeroDrainPerSecond => _heroDrainPerSecond;
        public int AvatarDrainPerSecond => _avatarDrainPerSecond;
        public float DrainTickInterval => _drainTickInterval;
        public float EscalationThreshold => _escalationThreshold;
        public int CountdownThreshold => _countdownThreshold;
    }
}