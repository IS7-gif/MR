using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "BattleTimerConfig", menuName = "Configs/Battle/Battle Timer Config")]
    public class BattleTimerConfig : ScriptableObject
    {
        [Tooltip("Общая длительность основной фазы боя в секундах")]
        [SerializeField] private float _battleDuration = 180f;

        [Tooltip("Оставшееся время в секундах, при котором начинается посекундный обратный отсчёт (например, 5 показывает 5, 4, 3, 2, 1)")]
        [SerializeField] private int _countdownThreshold = 5;


        public float BattleDuration => _battleDuration;
        public int CountdownThreshold => _countdownThreshold;
    }
}