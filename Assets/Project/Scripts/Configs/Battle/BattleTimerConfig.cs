using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "BattleTimerConfig", menuName = "Configs/Battle/Battle Timer Config")]
    public class BattleTimerConfig : ScriptableObject
    {
        [Tooltip("Total battle duration in seconds before Overtime begins")]
        [SerializeField] private float _battleDuration = 180f;
        
        [Tooltip("Remaining time threshold in seconds at which the battle escalation phase begins - triggers announcement and gameplay modifiers")]
        [SerializeField] private float _escalationThreshold = 15f;

        [Tooltip("Fixed damage per second applied to each hero during Overtime - higher current HP means longer survival")]
        [SerializeField] private int _heroDrainPerSecond = 200;

        [Tooltip("Fixed damage per second applied to an exposed avatar during Overtime")]
        [SerializeField] private int _avatarDrainPerSecond = 150;

        [Tooltip("How often drain damage ticks are applied in seconds - lower values produce smoother HP bar animation")]
        [SerializeField] private float _drainTickInterval = 0.1f;


        public float BattleDuration => _battleDuration;
        public int HeroDrainPerSecond => _heroDrainPerSecond;
        public int AvatarDrainPerSecond => _avatarDrainPerSecond;
        public float DrainTickInterval => _drainTickInterval;
        public float EscalationThreshold => _escalationThreshold;
    }
}